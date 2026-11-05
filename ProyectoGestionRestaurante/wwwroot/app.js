"use strict";

// ---------------------------------------------------------------
//  Pantalla del restaurante. Habla con la API (/api/...) usando el
//  token que devuelve el inicio de sesión.
//  La sesión vive solo en memoria: si se recarga la página, se vuelve a entrar.
// ---------------------------------------------------------------

let sesion = null;          // { token, id, nombre, tipo }
let ultimoCorte = null;     // para mostrar el resumen del último corte
let temporizador = null;

const TIPOS_PEDIDO = {
  ComerLocal: "Comer en el local",
  Domicilio: "Domicilio",
  RecogerTienda: "Recoger en la tienda"
};

// ---------- Utilidades ----------

const $ = id => document.getElementById(id);
const esCajero = () => sesion.tipo === "Cajero";
const esSupervisor = () => sesion.tipo === "Supervisor";
const fechaHoy = () => new Date().toLocaleDateString("en-CA");   // AAAA-MM-DD
const fechaHoraTexto = valor => new Intl.DateTimeFormat("es-CO", {
  dateStyle: "short", timeStyle: "medium"
}).format(new Date(valor));

const dinero = n => new Intl.NumberFormat("es-CO", {
  style: "currency", currency: "COP", maximumFractionDigits: 0
}).format(Number(n) || 0);

// Evita que un texto con símbolos raros rompa la pantalla.
const esc = t => String(t ?? "").replace(/[&<>"']/g, c =>
  ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));

function horasTexto(h) {
  const horas = Math.floor(h);
  const minutos = Math.round((h - horas) * 60);
  return `${horas} h ${minutos} min`;
}

function aviso(texto, tipo = "ok") {
  const caja = $("mensaje");
  caja.textContent = texto;
  caja.className = tipo;          // ok, error o aviso
  caja.hidden = false;
  clearTimeout(temporizador);
  temporizador = setTimeout(() => { caja.hidden = true; }, tipo === "error" ? 9000 : 6000);
}

// Ejecuta una acción y, si falla, muestra el mensaje en rojo.
async function intentar(accion) {
  try {
    await accion();
  } catch (e) {
    aviso(e.message, "error");
  }
}

function fallo(mensaje, estado) {
  const error = new Error(mensaje);
  error.estado = estado;
  return error;
}

// Llamada a la API. Devuelve los datos o lanza un Error con un mensaje claro.
async function llamar(metodo, ruta, cuerpo) {
  const opciones = { method: metodo, headers: {} };
  if (sesion) opciones.headers["Authorization"] = "Bearer " + sesion.token;
  if (cuerpo !== undefined) {
    opciones.headers["Content-Type"] = "application/json";
    opciones.body = JSON.stringify(cuerpo);
  }

  let respuesta;
  try {
    respuesta = await fetch("/api" + ruta, opciones);
  } catch {
    throw fallo("No se pudo conectar con el servidor.", 0);
  }

  const texto = await respuesta.text();
  let datos = texto;
  try { datos = texto ? JSON.parse(texto) : null; } catch { /* era texto simple */ }

  if (respuesta.ok) return datos;

  if (respuesta.status === 401 && sesion) {
    cerrarSesion();
    throw fallo("Tu sesión venció. Entra de nuevo.", 401);
  }
  if (respuesta.status === 403) throw fallo("No tienes permiso para hacer esto.", 403);
  if (typeof datos === "string" && datos) throw fallo(datos, respuesta.status);
  throw fallo("Revisa los datos e inténtalo de nuevo.", respuesta.status);
}

// Ventanita para pedir un número. Devuelve el número o null si cancela.
function pedirNumero(texto, valorInicial = "") {
  return new Promise(resolver => {
    const ventana = $("dialogo");
    $("dlgTexto").textContent = texto;
    $("dlgInput").value = valorInicial;
    ventana.returnValue = "";
    ventana.onclose = () =>
      resolver(ventana.returnValue === "ok" ? Number($("dlgInput").value) : null);
    ventana.showModal();
    $("dlgInput").focus();
  });
}

// ---------- Inicio y cierre de sesión ----------

$("formLogin").addEventListener("submit", e => {
  e.preventDefault();
  intentar(async () => {
    sesion = null;
    const r = await llamar("POST", "/auth/login", {
      nombre: $("loginNombre").value,
      contrasena: $("loginClave").value
    });
    sesion = { token: r.token, id: r.id, nombre: r.nombre, tipo: r.tipo };
    $("formLogin").reset();
    entrar();
  });
});

$("btnSalir").addEventListener("click", cerrarSesion);

function entrar() {
  $("vistaLogin").hidden = true;
  $("vistaApp").hidden = false;
  $("encabezado").hidden = false;
  $("usuario").textContent = `${sesion.nombre} (${sesion.tipo})`;

  const permitidas = PESTANAS.filter(p => p.roles.includes(sesion.tipo));
  $("menu").innerHTML = permitidas
    .map(p => `<button data-id="${p.id}">${p.titulo}</button>`)
    .join("");
  abrirPestana(permitidas[0].id);
}

function cerrarSesion() {
  sesion = null;
  ultimoCorte = null;
  $("vistaLogin").hidden = false;
  $("vistaApp").hidden = true;
  $("encabezado").hidden = true;
  $("contenido").innerHTML = "";
}

// ---------- Menú ----------

const PESTANAS = [
  { id: "pedidos",   titulo: "🧾 Pedidos",   roles: ["Cajero", "Supervisor"], vista: vistaPedidos },
  { id: "caja",      titulo: "💵 Caja",      roles: ["Cajero", "Supervisor"], vista: vistaCaja },
  { id: "gastos",    titulo: "🧺 Gastos",    roles: ["Cajero", "Supervisor"], vista: vistaGastos },
  { id: "turnos",    titulo: "⏰ Turnos",    roles: ["Cajero", "Supervisor"], vista: vistaTurnos },
  { id: "productos", titulo: "📋 Productos", roles: ["Supervisor"],           vista: vistaProductos },
  { id: "empleados", titulo: "👥 Empleados", roles: ["Supervisor"],           vista: vistaEmpleados }
];

$("menu").addEventListener("click", e => {
  const boton = e.target.closest("button");
  if (boton) abrirPestana(boton.dataset.id);
});

async function abrirPestana(id) {
  document.querySelectorAll("#menu button")
    .forEach(b => b.classList.toggle("activa", b.dataset.id === id));
  $("contenido").innerHTML = '<p class="vacio">Cargando…</p>';

  try {
    await PESTANAS.find(p => p.id === id).vista();
  } catch (e) {
    $("contenido").innerHTML = "";
    aviso(e.message, "error");
  }
}

// =====================================================================
//  PEDIDOS
// =====================================================================

async function vistaPedidos(pedidoEdicion = null) {
  const productos = await llamar("GET", "/productos");
  const pedidos = await llamar("GET", "/pedidos");

  const formulario = productos.length === 0
    ? '<p class="vacio">Todavía no hay productos. Pídele al supervisor que cree el menú.</p>'
    : `
      <label>Tipo de pedido
        <select id="pTipo">
          ${Object.entries(TIPOS_PEDIDO).map(([valor, texto]) => `<option value="${valor}">${texto}</option>`).join("")}
        </select>
      </label>
      <div id="camposTipo"></div>

      <h3>Productos</h3>
      <div id="filasProductos"></div>
      <button type="button" id="btnMasProducto" class="secundario">+ Agregar otro producto</button>
      <p class="total">Total de los productos: <b id="pTotal">$0</b></p>
      <div class="botones-fila">
        <button id="btnCrearPedido">Crear pedido</button>
        <button type="button" id="btnCancelarEdicion" class="secundario" hidden>Cancelar edición</button>
      </div>`;

  $("contenido").innerHTML = `
    <div class="tarjeta">
      <h2 id="tituloPedido">Nuevo pedido</h2>
      ${formulario}
    </div>
    <div class="tarjeta">
      <h2>Pedidos</h2>
      <div class="tabla-caja" id="tablaPedidos">${tablaPedidos(pedidos)}</div>
    </div>`;

  $("tablaPedidos").addEventListener("click", e => {
    const cobrarBtn = e.target.closest(".cobrar");
    const editarBtn = e.target.closest(".editar-pedido");
    const facturaBtn = e.target.closest(".ver-factura");
    if (cobrarBtn) intentar(() => cobrar(cobrarBtn.dataset));
    if (editarBtn) intentar(async () => {
      const pedido = await llamar("GET", `/pedidos/${editarBtn.dataset.id}`);
      await vistaPedidos(pedido);
      $("contenido").scrollIntoView({ behavior: "smooth" });
    });
    if (facturaBtn) intentar(() => verFactura(Number(facturaBtn.dataset.numero)));
  });

  if (productos.length > 0) prepararFormularioPedido(productos, pedidoEdicion);
}

function prepararFormularioPedido(productos, pedidoEdicion = null) {
  const opciones = productos.map(p =>
    `<option value="${p.id}" data-precio="${p.precio}">${esc(p.nombre)} — ${dinero(p.precio)}</option>`
  ).join("");
  const filas = $("filasProductos");
  let pedidoEnEdicion = pedidoEdicion?.id ?? null;

  function pintarCamposTipo() {
    const tipo = $("pTipo").value;
    if (tipo === "ComerLocal") {
      $("camposTipo").innerHTML = `<label>Número de mesa<input id="pMesa" type="number" min="1"></label>`;
    } else if (tipo === "Domicilio") {
      $("camposTipo").innerHTML = `
        <div class="dos">
          <label>Dirección<input id="pDireccion"></label>
          <label>Teléfono<input id="pTelefono" type="tel"></label>
          <label>Costo del envío<input id="pEnvio" type="number" min="0" value="0"></label>
        </div>`;
    } else {
      $("camposTipo").innerHTML = `<label>Nombre del cliente<input id="pCliente"></label>`;
    }
  }

  function recalcular() {
    let total = 0;
    filas.querySelectorAll(".fila").forEach(fila => {
      const precio = Number(fila.querySelector(".selProd").selectedOptions[0].dataset.precio);
      total += precio * (Number(fila.querySelector(".cant").value) || 0);
    });
    $("pTotal").textContent = dinero(total);
  }

  function agregarFila(item = null) {
    const fila = document.createElement("div");
    fila.className = "fila";
    fila.innerHTML = `
      <label class="crece">Producto<select class="selProd">${opciones}</select></label>
      <label class="chica">Cantidad<input class="cant" type="number" min="1" value="${item?.cantidad ?? 1}"></label>
      <button type="button" class="quitar secundario" title="Quitar este producto">✕</button>`;
    filas.appendChild(fila);
    if (item) fila.querySelector(".selProd").value = item.productoId;
    recalcular();
  }

  $("pTipo").addEventListener("change", pintarCamposTipo);
  $("btnMasProducto").addEventListener("click", () => agregarFila());
  filas.addEventListener("input", recalcular);
  filas.addEventListener("change", recalcular);
  filas.addEventListener("click", e => {
    if (!e.target.classList.contains("quitar")) return;
    if (filas.children.length === 1) {
      aviso("El pedido debe tener al menos un producto.", "aviso");
      return;
    }
    e.target.closest(".fila").remove();
    recalcular();
  });

  $("btnCrearPedido").addEventListener("click", () => intentar(async () => {
    const tipo = $("pTipo").value;
    const items = [...filas.querySelectorAll(".fila")].map(fila => ({
      productoId: Number(fila.querySelector(".selProd").value),
      cantidad: Number(fila.querySelector(".cant").value)
    }));

    const cuerpo = { tipo, items };
    if (tipo === "ComerLocal") {
      cuerpo.mesa = Number($("pMesa").value) || null;
    } else if (tipo === "Domicilio") {
      cuerpo.direccion = $("pDireccion").value;
      cuerpo.telefono = $("pTelefono").value;
      cuerpo.costoEnvio = Number($("pEnvio").value) || 0;
    } else {
      cuerpo.nombreCliente = $("pCliente").value;
    }

    const pedido = pedidoEnEdicion
      ? await llamar("PUT", `/pedidos/${pedidoEnEdicion}`, cuerpo)
      : await llamar("POST", "/pedidos", cuerpo);
    aviso(pedidoEnEdicion
      ? `Pedido #${pedido.id} actualizado. Total: ${dinero(pedido.total)}.`
      : `Pedido #${pedido.id} creado. Total: ${dinero(pedido.total)}.`);
    await vistaPedidos();
  }));

  $("btnCancelarEdicion").addEventListener("click", () => intentar(vistaPedidos));

  if (pedidoEdicion) {
    $("tituloPedido").textContent = `Modificar pedido #${pedidoEdicion.id}`;
    $("btnCrearPedido").textContent = "Guardar cambios";
    $("btnCancelarEdicion").hidden = false;
    $("pTipo").value = pedidoEdicion.tipo;
    $("pTipo").disabled = true;
  }
  pintarCamposTipo();
  if (pedidoEdicion) {
    if (pedidoEdicion.tipo === "ComerLocal") $("pMesa").value = pedidoEdicion.mesa;
    if (pedidoEdicion.tipo === "Domicilio") {
      $("pDireccion").value = pedidoEdicion.direccion;
      $("pTelefono").value = pedidoEdicion.telefono;
      $("pEnvio").value = pedidoEdicion.costoEnvio;
    }
    if (pedidoEdicion.tipo === "RecogerTienda") $("pCliente").value = pedidoEdicion.nombreCliente;
    pedidoEdicion.detalles.filter(d => d.productoId != null).forEach(agregarFila);
  }
}

function tablaPedidos(pedidos) {
  if (pedidos.length === 0) return '<p class="vacio">Todavía no hay pedidos.</p>';

  const filas = [...pedidos].sort((a, b) => b.id - a.id).map(p => {
    const acciones = [
      !p.cobrado && esCajero()
        ? `<button class="cobrar" data-id="${p.id}" data-venta="${p.ventaId}" data-total="${p.total}">Cobrar</button>`
        : "",
      !p.cobrado ? `<button class="editar-pedido secundario" data-id="${p.id}">Editar</button>` : "",
      p.facturaNumero ? `<button class="ver-factura secundario" data-numero="${p.facturaNumero}">Ver factura</button>` : ""
    ].filter(Boolean).join(" ");
    const estado = p.cobrado
      ? '<span class="estado-ok">Cobrado</span>'
      : '<span class="estado-pendiente">Pendiente de cobro</span>';

    return `<tr>
      <td>#${p.id}</td>
      <td>${TIPOS_PEDIDO[p.tipo] ?? esc(p.tipo)}</td>
      <td>${esc(p.fecha)}<br>${esc(p.horaPedido)}</td>
      <td>${detallePedido(p)}</td>
      <td>${dinero(p.total)}</td>
      <td>${estado}</td>
      <td>${acciones}</td>
    </tr>`;
  }).join("");

  return `<table>
    <thead><tr><th>N.º</th><th>Tipo</th><th>Fecha y hora</th><th>Detalle</th><th>Total</th><th>Estado</th><th></th></tr></thead>
    <tbody>${filas}</tbody></table>`;
}

function detallePedido(p) {
  const productos = p.detalles.map(d => `${d.cantidad} × ${esc(d.descripcion)}`).join("<br>");
  let extra = "";
  if (p.mesa) extra = `Mesa ${p.mesa}`;
  if (p.tipo === "Domicilio") extra = `${esc(p.direccion)} · Tel. ${esc(p.telefono)} · Envío ${dinero(p.costoEnvio)}`;
  if (p.nombreCliente) extra = `Cliente: ${esc(p.nombreCliente)}`;
  return productos + (extra ? `<br><small class="vacio">${extra}</small>` : "");
}

async function cobrar(datos) {
  const recibido = await pedirNumero(
    `Pedido #${datos.id} — Total a pagar: ${dinero(datos.total)}. ¿Cuánto dinero recibió?`,
    datos.total);
  if (recibido === null) return;

  const factura = await llamar("POST", "/caja/cobrar", {
    ventaId: Number(datos.venta),
    dineroRecibido: recibido
  });
  aviso(`Cobrado. Factura #${factura.numero}. Cambio a devolver: ${dinero(factura.cambio)}.`);
  mostrarFactura(factura);
  await vistaPedidos();
}

async function verFactura(numero) {
  const factura = await llamar("GET", `/caja/factura/${numero}`);
  mostrarFactura(factura);
}

function mostrarFactura(f) {
  const filas = f.detalles.map(d => `<tr><td>${esc(d.descripcion)}</td><td>${d.cantidad}</td><td>${dinero(d.precioUnitario)}</td><td>${dinero(d.subtotal)}</td></tr>`).join("");
  mostrarComprobante(`Factura #${f.numero}`, `
    <p><b>Pedido:</b> ${f.pedidoId ? `#${f.pedidoId}` : "—"} ${f.tipoPedido ? `(${esc(TIPOS_PEDIDO[f.tipoPedido] ?? f.tipoPedido)})` : ""}</p>
    <p><b>Fecha y hora del pedido:</b> ${f.fechaPedido ? esc(f.fechaPedido) : "—"} ${f.horaPedido ? esc(f.horaPedido) : ""}</p>
    <p><b>Fecha y hora del pago:</b> ${esc(fechaHoraTexto(f.fecha))} · ${esc(f.cajero)}</p>
    <div class="tabla-caja"><table><thead><tr><th>Producto</th><th>Cant.</th><th>Precio</th><th>Subtotal</th></tr></thead><tbody>${filas}</tbody></table></div>
    <p>Subtotal: <b>${dinero(f.subtotal)}</b>${f.descuento ? ` · Descuento: ${f.descuento}%` : ""}</p>
    <p>Total: <b>${dinero(f.total)}</b> · Recibido: ${dinero(f.valorPagado)} · Cambio: ${dinero(f.cambio)}</p>`);
}

function mostrarComprobante(titulo, contenido) {
  $("tituloComprobante").textContent = titulo;
  $("contenidoComprobante").innerHTML = contenido;
  $("dialogoComprobante").showModal();
}

// =====================================================================
//  CAJA
// =====================================================================

async function vistaCaja() {
  // Si la caja está cerrada, la API responde 400: eso no es un fallo, es el estado.
  let saldo = null;
  try {
    saldo = await llamar("GET", "/caja/saldo");
  } catch (e) {
    if (e.estado !== 400) throw e;
  }

  const cortes = await llamar("GET", "/corte");

  const estado = saldo
    ? `<div class="resumen">
         <div class="dato"><span>Base inicial</span><b>${dinero(saldo.baseInicial)}</b></div>
         <div class="dato"><span>Vendido hasta ahora</span><b>${dinero(saldo.totalVentas)}</b></div>
         <div class="dato"><span>Gastos</span><b>${dinero(saldo.totalGastos)}</b></div>
         <div class="dato grande"><span>Dinero que debe haber en la caja</span><b>${dinero(saldo.saldo)}</b></div>
       </div>`
    : '<p class="vacio">La caja está cerrada.</p>';

  const abrir = !saldo && esCajero()
    ? `<div class="tarjeta">
         <h2>Abrir caja</h2>
         <label>Base inicial (dinero con el que empieza la caja)
           <input id="baseInicial" type="number" min="0" placeholder="Ejemplo: 50000">
         </label>
         <button id="btnAbrir">Abrir caja</button>
       </div>`
    : (!saldo ? '<p class="vacio">Un cajero debe abrir la caja.</p>' : "");

  const corte = saldo
    ? `<div class="tarjeta">
         <h2>Corte de caja</h2>
         <p class="ayuda">El corte parcial deja la caja abierta y no exige cerrar los turnos. Para finalizar el día, usa el cierre final.</p>
         <div class="botones-fila">
           <button id="btnCorteParcial">Hacer corte parcial</button>
           <button id="btnCierreCaja" class="peligro">Hacer corte y cerrar caja</button>
         </div>
       </div>`
    : "";
  const historial = tarjetaHistorialCortes(cortes);

  $("contenido").innerHTML = `
    <div class="tarjeta"><h2>Estado de la caja</h2>${estado}</div>
    ${abrir}
    ${corte}
    ${historial}`;

  if ($("btnAbrir")) {
    $("btnAbrir").addEventListener("click", () => intentar(async () => {
      const base = $("baseInicial").value;
      if (base === "") {
        aviso("Escribe la base inicial (puede ser 0).", "aviso");
        return;
      }
      const r = await llamar("POST", "/caja/abrir", { baseInicial: Number(base) });
      ultimoCorte = null;
      await vistaCaja();
      aviso(`Caja abierta. ${r.aviso}`, "aviso");
    }));
  }

  if ($("btnCorteParcial")) {
    $("btnCorteParcial").addEventListener("click", () => intentar(async () => {
      const corte = await llamar("POST", "/corte?cerrarCaja=false");
      mostrarCorte(corte);
      await vistaCaja();
    }));
    $("btnCierreCaja").addEventListener("click", () => {
      if (!confirm("¿Hacer el corte final y cerrar la caja?")) return;
      intentar(async () => {
        const corte = await llamar("POST", "/corte?cerrarCaja=true");
        mostrarCorte(corte);
        await vistaCaja();
      });
    });
  }

  $("tablaCortes")?.addEventListener("click", e => {
    const boton = e.target.closest(".ver-corte");
    if (boton) {
      const corte = cortes[Number(boton.dataset.indice)];
      mostrarCorte(corte);
    }
  });
}

function tarjetaHistorialCortes(cortes) {
  if (cortes.length === 0) return `<div class="tarjeta"><h2>Cortes anteriores</h2><p class="vacio">Todavía no hay cortes.</p></div>`;
  const filas = [...cortes].reverse().map((c, i) => {
    const indice = cortes.length - 1 - i;
    return `<tr><td>${esc(c.fecha)} ${esc(c.horaCorte)}</td><td>${c.aperturaId}</td><td>${c.cajaCerrada ? "Cierre final" : "Parcial"}</td><td>${dinero(c.totalEsperado)}</td><td><button class="ver-corte secundario" data-indice="${indice}">Ver</button></td></tr>`;
  }).join("");
  return `<div class="tarjeta"><h2>Cortes anteriores</h2><div class="tabla-caja"><table id="tablaCortes"><thead><tr><th>Fecha y hora</th><th>Caja</th><th>Tipo</th><th>Saldo esperado</th><th></th></tr></thead><tbody>${filas}</tbody></table></div></div>`;
}

function mostrarCorte(c) {
  mostrarComprobante(`Corte de caja · ${esc(c.fecha)} ${esc(c.horaCorte)}`, `
    <p><b>Estado:</b> ${c.cajaCerrada ? "Caja cerrada" : "Corte parcial; caja abierta"}</p>
    <p><b>Hora de inicio:</b> ${esc(c.horaInicioTurno)} · <b>Apertura:</b> ${c.aperturaId}</p>
    <div class="resumen">
      <div class="dato"><span>Base inicial</span><b>${dinero(c.baseInicial)}</b></div>
      <div class="dato"><span>Total de gastos</span><b>${dinero(c.totalGastos)}</b></div>
      <div class="dato"><span>Total de ventas</span><b>${dinero(c.totalVentas)}</b></div>
      <div class="dato grande"><span>Saldo esperado</span><b>${dinero(c.totalEsperado)}</b></div>
    </div>`);
}

// =====================================================================
//  GASTOS
// =====================================================================

async function vistaGastos() {
  const lista = esSupervisor()
    ? `<div class="tarjeta">
         <h2>Gastos del día</h2>
         <label>Fecha<input id="gFecha" type="date" value="${fechaHoy()}"></label>
         <div class="tabla-caja" id="tablaGastos"></div>
       </div>`
    : "";

  $("contenido").innerHTML = `
    <div class="tarjeta">
      <h2>Registrar un gasto</h2>
      <div class="dos">
        <label>¿En qué se gastó?<input id="gDescripcion" placeholder="Ejemplo: hielo"></label>
        <label>Valor<input id="gValor" type="number" min="1"></label>
      </div>
      <button id="btnGasto">Guardar gasto</button>
    </div>
    ${lista}`;

  $("btnGasto").addEventListener("click", () => intentar(async () => {
    const gasto = await llamar("POST", "/gastos", {
      descripcion: $("gDescripcion").value,
      valor: Number($("gValor").value) || 0
    });
    aviso("Gasto guardado.");
    mostrarGasto(gasto);
    $("gDescripcion").value = "";
    $("gValor").value = "";
    if (esSupervisor()) await cargarGastos();
  }));

  if (esSupervisor()) {
    $("gFecha").addEventListener("change", () => intentar(cargarGastos));
    await cargarGastos();
  }
}

async function cargarGastos() {
  const gastos = await llamar("GET", `/gastos?fecha=${$("gFecha").value}`);
  if (gastos.length === 0) {
    $("tablaGastos").innerHTML = '<p class="vacio">No hay gastos en esta fecha.</p>';
    return;
  }

  const total = gastos.reduce((suma, g) => suma + g.valor, 0);
  const filas = gastos.map(g => `<tr data-id="${g.id}">
    <td>${esc(g.hora)}</td>
    <td><input class="g-descripcion" value="${esc(g.descripcion)}"></td>
    <td><input class="g-valor" type="number" min="1" value="${g.valor}"></td>
    <td>${esc(g.empleado)}</td>
    <td><div class="botones-fila"><button class="guardar-gasto secundario">Guardar</button><button class="ver-gasto secundario">Ver</button></div></td>
  </tr>`).join("");

  $("tablaGastos").innerHTML = `<table>
    <thead><tr><th>Hora</th><th>Descripción</th><th>Valor</th><th>Registró</th><th></th></tr></thead>
    <tbody>${filas}</tbody></table>
    <p class="total">Total: <b>${dinero(total)}</b></p>`;
  $("tablaGastos").addEventListener("click", e => {
    const fila = e.target.closest("tr[data-id]");
    if (!fila) return;
    if (e.target.closest(".ver-gasto")) {
      const gasto = gastos.find(g => g.id === Number(fila.dataset.id));
      mostrarGasto(gasto);
    }
    if (e.target.closest(".guardar-gasto")) intentar(async () => {
      await llamar("PUT", `/gastos/${fila.dataset.id}`, {
        descripcion: fila.querySelector(".g-descripcion").value,
        valor: Number(fila.querySelector(".g-valor").value) || 0
      });
      aviso("Gasto actualizado.");
      await cargarGastos();
    });
  });
}

function mostrarGasto(g) {
  mostrarComprobante(`Comprobante de gasto #${g.id}`, `
    <p><b>Fecha y hora:</b> ${esc(g.fecha)} · ${esc(g.hora)}</p>
    <p><b>Descripción:</b> ${esc(g.descripcion)}</p>
    <p><b>Valor:</b> ${dinero(g.valor)}</p>
    <p><b>Registró:</b> ${esc(g.empleado)}</p>`);
}

// =====================================================================
//  TURNOS (horas de los trabajadores)
// =====================================================================

async function vistaTurnos() {
  let quien;
  if (esSupervisor()) {
    const empleados = await llamar("GET", "/empleados");
    quien = `<label>Trabajador
      <select id="tEmpleado">
        ${empleados.map(e => `<option value="${e.id}">${esc(e.nombre)} (${e.tipo})</option>`).join("")}
      </select></label>`;
  } else {
    quien = `<p>Trabajador: <b>${esc(sesion.nombre)}</b></p>`;
  }

  $("contenido").innerHTML = `
    <div class="tarjeta">
      <h2>Turnos de trabajo</h2>
      ${quien}
      <div class="botones-fila">
        <button id="btnEntrada">▶ Iniciar turno</button>
        <button id="btnSalida" class="secundario">⏹ Terminar turno</button>
      </div>
    </div>
    <div id="resultadoTurnos"></div>`;

  const empleadoId = () => esSupervisor() ? Number($("tEmpleado").value) : sesion.id;

  $("btnEntrada").addEventListener("click", () => intentar(async () => {
    const r = await llamar("POST", "/horas/entrada", { empleadoId: empleadoId() });
    aviso(`Turno iniciado: ${r.empleado}, a las ${r.horaEntrada}.`);
    await verHoras(empleadoId());
  }));

  $("btnSalida").addEventListener("click", () => intentar(async () => {
    const r = await llamar("POST", "/horas/salida", { empleadoId: empleadoId() });
    aviso(`Turno terminado: ${r.empleado} trabajó ${horasTexto(r.horasTrabajadas)} (${r.horasTrabajadas} horas).`);
    await verHoras(empleadoId());
  }));

  if (esSupervisor()) {
    $("tEmpleado").addEventListener("change", () => intentar(() => verHoras(empleadoId())));
  }

  await verHoras(empleadoId());
}

async function verHoras(empleadoId) {
  const h = await llamar("GET", `/horas/${empleadoId}`);

  const filas = [...h.registros].reverse().map(r => `<tr>
    <td>${esc(r.fecha)}</td>
    <td>${esc(r.horaEntrada)}</td>
    <td>${r.horaSalida ? esc(r.horaSalida) : '<span class="estado-pendiente">En curso</span>'}</td>
    <td>${r.horaSalida ? horasTexto(r.horasTrabajadas) : "—"}</td>
  </tr>`).join("");

  $("resultadoTurnos").innerHTML = `
    <div class="tarjeta">
      <h2>Horas de ${esc(h.nombre)}</h2>
      <div class="resumen">
        <div class="dato grande"><span>Total de horas trabajadas</span><b>${horasTexto(h.horasTrabajadas)} (${h.horasTrabajadas} horas)</b></div>
        <div class="dato"><span>Turnos terminados</span><b>${h.registrosCerrados}</b></div>
        <div class="dato"><span>Turno actual</span><b>${h.tieneEntradaAbierta ? "En curso" : "Ninguno"}</b></div>
      </div>
      ${h.registros.length === 0
        ? '<p class="vacio">Todavía no hay turnos registrados.</p>'
        : `<h3>Turnos registrados</h3><div class="tabla-caja"><table>
             <thead><tr><th>Fecha</th><th>Entrada</th><th>Salida</th><th>Horas</th></tr></thead>
             <tbody>${filas}</tbody></table></div>`}
    </div>`;
}

// =====================================================================
//  PRODUCTOS (solo supervisor)
// =====================================================================

async function vistaProductos() {
  const productos = await llamar("GET", "/productos");

  const filas = productos.map(p => `<tr data-id="${p.id}">
    <td><input class="nombre" value="${esc(p.nombre)}"></td>
    <td><input class="precio" type="number" min="1" value="${p.precio}"></td>
    <td><button class="guardar secundario">Guardar</button></td>
  </tr>`).join("");

  $("contenido").innerHTML = `
    <div class="tarjeta">
      <h2>Nuevo producto</h2>
      <div class="dos">
        <label>Nombre<input id="prNombre" placeholder="Ejemplo: Hamburguesa"></label>
        <label>Precio<input id="prPrecio" type="number" min="1"></label>
      </div>
      <button id="btnProducto">Crear producto</button>
    </div>
    <div class="tarjeta">
      <h2>Menú</h2>
      ${productos.length === 0
        ? '<p class="vacio">Todavía no hay productos.</p>'
        : `<p class="ayuda">Para cambiar un nombre o un precio, edítalo y pulsa Guardar.</p>
           <div class="tabla-caja"><table id="tablaProductos">
             <thead><tr><th>Nombre</th><th>Precio</th><th></th></tr></thead>
             <tbody>${filas}</tbody></table></div>`}
    </div>`;

  $("btnProducto").addEventListener("click", () => intentar(async () => {
    await llamar("POST", "/productos", {
      nombre: $("prNombre").value,
      precio: Number($("prPrecio").value) || 0
    });
    aviso("Producto creado.");
    await vistaProductos();
  }));

  if ($("tablaProductos")) {
    $("tablaProductos").addEventListener("click", e => {
      const boton = e.target.closest(".guardar");
      if (!boton) return;
      const fila = boton.closest("tr");
      intentar(async () => {
        await llamar("PUT", `/productos/${fila.dataset.id}`, {
          nombre: fila.querySelector(".nombre").value,
          precio: Number(fila.querySelector(".precio").value) || 0
        });
        aviso("Producto actualizado.");
      });
    });
  }
}

// =====================================================================
//  EMPLEADOS (solo supervisor)
// =====================================================================

async function vistaEmpleados() {
  const empleados = await llamar("GET", "/empleados");

  const filas = empleados.map(e => `<tr>
    <td>${e.id}</td><td>${esc(e.nombre)}</td><td>${esc(e.tipo)}</td>
  </tr>`).join("");

  $("contenido").innerHTML = `
    <div class="tarjeta">
      <h2>Nuevo empleado</h2>
      <div class="dos">
        <label>Nombre<input id="emNombre" autocomplete="off"></label>
        <label>Cargo
          <select id="emTipo"><option>Cajero</option><option>Supervisor</option></select>
        </label>
        <label>Contraseña (mínimo 6 caracteres)<input id="emClave" type="password" autocomplete="new-password"></label>
      </div>
      <button id="btnEmpleado">Crear empleado</button>
    </div>
    <div class="tarjeta">
      <h2>Empleados</h2>
      <div class="tabla-caja"><table>
        <thead><tr><th>N.º</th><th>Nombre</th><th>Cargo</th></tr></thead>
        <tbody>${filas}</tbody></table></div>
    </div>`;

  $("btnEmpleado").addEventListener("click", () => intentar(async () => {
    await llamar("POST", "/empleados", {
      nombre: $("emNombre").value,
      tipo: $("emTipo").value,
      contrasena: $("emClave").value
    });
    aviso("Empleado creado.");
    await vistaEmpleados();
  }));
}

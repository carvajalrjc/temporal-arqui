// src/Frontend/src/services/appService.ts
import { ACTIONS, callSoap, escapeXml, firstByLocalName } from "../lib/soap";

const NS = "urn:mini-facturacion:v1";

export type Cliente = { id: number; nombres: string; documento: string; email: string; };
export type PageClientes = { items: Cliente[]; total: number; };

export async function crearCliente(nuevo: Omit<Cliente, "id">) {
  const xml = `
  <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:v1="${NS}">
    <soap:Body>
      <v1:CrearCliente>
        <v1:nuevo>
          <v1:Id>0</v1:Id>
          <v1:Nombres>${escapeXml(nuevo.nombres)}</v1:Nombres>
          <v1:Documento>${escapeXml(nuevo.documento)}</v1:Documento>
          <v1:Email>${escapeXml(nuevo.email)}</v1:Email>
        </v1:nuevo>
      </v1:CrearCliente>
    </soap:Body>
  </soap:Envelope>`.trim();

  const doc = await callSoap(ACTIONS.CrearCliente, xml);
  // La respuesta suele traer un nodo ClienteDto con sus campos
  const id = Number(firstByLocalName(doc, "Id")?.textContent ?? "0");
  return { id };
}

export async function listarClientes(pagina = 1, tam = 10, q = ""): Promise<PageClientes> {
  const reqQ = q ? `<v1:Q>${escapeXml(q)}</v1:Q>` : "";
  const xml = `
  <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:v1="${NS}">
    <soap:Body>
      <v1:ListarClientes>
        <v1:req>
          <v1:Pagina>${pagina}</v1:Pagina>
          <v1:Tam>${tam}</v1:Tam>
          ${reqQ}
        </v1:req>
      </v1:ListarClientes>
    </soap:Body>
  </soap:Envelope>`.trim();

  const doc = await callSoap(ACTIONS.ListarClientes, xml);

  // PageResponse<ClienteDto> => Items + Total
  const itemsNode = firstByLocalName(doc, "Items");
  const itemEls = itemsNode
    ? Array.from(itemsNode.getElementsByTagName("*")).filter(n => n.localName === "ClienteDto")
    : [];

  const items: Cliente[] = itemEls.map(el => {
    const get = (name: string) => firstByLocalName(el, name)?.textContent ?? "";
    return {
      id: Number(get("Id") || "0"),
      nombres: get("Nombres"),
      documento: get("Documento"),
      email: get("Email"),
    };
  });

  const total = Number(firstByLocalName(doc, "Total")?.textContent ?? "0");
  return { items, total };
}

// ======= Facturas =======
export type Factura = { id: number; clienteId: number; fechaIso: string; monto: number; moneda: string; };

export async function crearFactura(f: Omit<Factura, "id">) {
  const xml = `
  <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:v1="${NS}">
    <soap:Body>
      <v1:CrearFactura>
        <v1:nueva>
          <v1:Id>0</v1:Id>
          <v1:ClienteId>${f.clienteId}</v1:ClienteId>
          <v1:FechaIso>${escapeXml(f.fechaIso)}</v1:FechaIso>
          <v1:Monto>${f.monto}</v1:Monto>
          <v1:Moneda>${escapeXml(f.moneda)}</v1:Moneda>
        </v1:nueva>
      </v1:CrearFactura>
    </soap:Body>
  </soap:Envelope>`.trim();

  const doc = await callSoap(ACTIONS.CrearFactura, xml);
  const id = Number(firstByLocalName(doc, "Id")?.textContent ?? "0");
  return { id };
}

export async function montoEnLetras(monto: number, moneda = "COP", cultura = "es-CO") {
  const xml = `
  <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:v1="${NS}">
    <soap:Body>
      <v1:MontoEnLetras>
        <v1:req>
          <v1:Monto>${monto}</v1:Monto>
          <v1:Moneda>${escapeXml(moneda)}</v1:Moneda>
          <v1:Cultura>${escapeXml(cultura)}</v1:Cultura>
        </v1:req>
      </v1:MontoEnLetras>
    </soap:Body>
  </soap:Envelope>`.trim();

  const doc = await callSoap(ACTIONS.MontoEnLetras, xml);
  const texto = firstByLocalName(doc, "Texto")?.textContent ?? "";
  return { texto };
}

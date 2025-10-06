// src/Frontend/src/lib/soap.ts
const SOAP_ENDPOINT =
  import.meta.env.PUBLIC_SOAP_ENDPOINT || "http://localhost:8080/soap/mini";
export { SOAP_ENDPOINT }; // opcional, por si lo quieres ver/loggear

const SOAP_NS = "urn:mini-facturacion:v1";

export const ACTIONS = {
  CrearCliente:        `${SOAP_NS}/IAppService/CrearCliente`,
  ObtenerCliente:      `${SOAP_NS}/IAppService/ObtenerCliente`,
  ActualizarCliente:   `${SOAP_NS}/IAppService/ActualizarCliente`,
  EliminarCliente:     `${SOAP_NS}/IAppService/EliminarCliente`,
  ListarClientes:      `${SOAP_NS}/IAppService/ListarClientes`,
  CrearFactura:        `${SOAP_NS}/IAppService/CrearFactura`,
  ObtenerFactura:      `${SOAP_NS}/IAppService/ObtenerFactura`,
  ActualizarFactura:   `${SOAP_NS}/IAppService/ActualizarFactura`,
  EliminarFactura:     `${SOAP_NS}/IAppService/EliminarFactura`,
  ListarFacturas:      `${SOAP_NS}/IAppService/ListarFacturas`,
  MontoEnLetras:       `${SOAP_NS}/IAppService/MontoEnLetras`,
} as const;

export function escapeXml(s: string) {
  return s
    .replace(/&/g,"&amp;")
    .replace(/</g,"&lt;")
    .replace(/>/g,"&gt;")
    .replace(/"/g,"&quot;")
    .replace(/'/g,"&apos;");
}

// Llama al gateway con SOAP/XML y devuelve el Document
export async function callSoap(action: string, body: string): Promise<Document> {
  const resp = await fetch(SOAP_ENDPOINT, {
    method: "POST",
    headers: {
      "Content-Type": "text/xml; charset=utf-8",
      "SOAPAction": `"${action}"`,
    },
    body,
    // mode: "cors", // opcional; el gateway ya está respondiendo CORS
  });

  const text = await resp.text();
  const doc = new DOMParser().parseFromString(text, "text/xml");

  const fault =
    doc.getElementsByTagNameNS("http://schemas.xmlsoap.org/soap/envelope/","Fault")[0] ||
    doc.getElementsByTagName("Fault")[0];

  if (fault) {
    const fs = fault.getElementsByTagName("faultstring")[0]?.textContent?.trim() ?? "SOAP Fault";
    throw new Error(fs);
  }
  return doc;
}

// helper: primer nodo por localName (ignora namespaces/prefijos)
export function firstByLocalName(docOrEl: Document | Element, local: string): Element | null {
  const all = (docOrEl as Document).getElementsByTagName
    ? (docOrEl as Document).getElementsByTagName("*")
    : (docOrEl as Element).getElementsByTagName("*");
  for (const n of Array.from(all)) {
    if (n.localName === local) return n as Element;
  }
  return null;
}

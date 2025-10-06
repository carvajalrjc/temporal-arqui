/// <reference types="astro/client" />

// Expón las vars públicas que usarás en el cliente
interface ImportMetaEnv {
  readonly PUBLIC_SOAP_ENDPOINT: string;
}

// Define env dentro de ImportMeta
interface ImportMeta {
  readonly env: ImportMetaEnv;
}

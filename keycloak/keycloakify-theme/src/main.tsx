import React from "react";
import { createRoot } from "react-dom/client";

function App() {
  return (
    <main style={{ fontFamily: "Inter, system-ui, sans-serif", padding: "2rem" }}>
      <h1>GateWise Keycloak Theme</h1>
      <p>
        Este projeto Vite existe para empacotar o tema nativo do Keycloak via
        Keycloakify. O tema de login fica em <code>src/keycloak-theme/login</code>.
      </p>
    </main>
  );
}

createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
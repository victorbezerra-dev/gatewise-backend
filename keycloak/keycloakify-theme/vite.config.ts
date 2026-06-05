import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { keycloakify } from "keycloakify/vite-plugin";

export default defineConfig({
  plugins: [
    react(),
    keycloakify({
      themeName: "gatewise",
      accountThemeImplementation: "none",
      extraThemeProperties: ["locales=en,pt-BR"],
      keycloakifyBuildDirPath: "../keycloakify-theme-build"
    })
  ]
});
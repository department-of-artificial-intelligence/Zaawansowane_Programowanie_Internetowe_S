import { defineConfig } from "vite";
import { transformSync } from "@babel/core";

export default defineConfig({
  plugins: [
    {
      name: "ts-stage3-decorators",
      enforce: "pre",
      transform(code, id) {
        const filePath = id.split("?")[0];
        if (!filePath.endsWith(".ts")) return null;

        const result = transformSync(code, {
          filename: filePath,
          presets: ["@babel/preset-typescript"],
          plugins: [
            ["@babel/plugin-proposal-decorators", { version: "2023-11" }],
          ],
        });

        if (!result || !result.code) return null;

        return {
          code: result.code,
          map: (result.map as any) ?? undefined,
        };
      },
    },
  ],
});

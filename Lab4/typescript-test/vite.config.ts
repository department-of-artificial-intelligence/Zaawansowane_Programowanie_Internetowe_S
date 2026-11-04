import { defineConfig, type Plugin } from "vite";
import { transform } from "esbuild";

// Vite 8 (transformator Oxc) nie tłumaczy standardowych dekoratorów (Stage 3)
// na kod zrozumiały dla przeglądarek. Ta wtyczka robi to za pomocą esbuild,
// tylko dla plików .ts, w których występują dekoratory.
function standardDecorators(): Plugin {
  return {
    name: "standard-decorators",
    enforce: "pre",
    async transform(code, id) {
      if (!id.endsWith(".ts") || !/@[A-Za-z_$]/.test(code)) return null;
      const result = await transform(code, {
        loader: "ts",
        target: "es2022",
        sourcemap: true,
        sourcefile: id,
        // te same semantyki pól klas co w TypeScript dla target ES2022+
        tsconfigRaw: { compilerOptions: { useDefineForClassFields: true } },
      });
      return { code: result.code, map: result.map };
    },
  };
}

export default defineConfig({
  plugins: [standardDecorators()],
});

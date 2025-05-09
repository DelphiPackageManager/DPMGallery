import react from "@vitejs/plugin-react";
import path from "path";
import { defineConfig } from "vite";
import svgr from "vite-plugin-svgr";
import topLevelAwait from "vite-plugin-top-level-await";

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [
		react(),
		svgr(),

		topLevelAwait({
			// The export name of top-level await promise for each chunk module
			promiseExportName: "__tla",
			// The function to generate import names of top-level await promise in each chunk module
			promiseImportName: (i) => `__tla_${i}`,
		}),
	],
	resolve: {
		alias: {
			"@": path.resolve(__dirname, "./src"),
		},
	},
	build: {
		emptyOutDir: true,
		outDir: "../wwwroot",
		manifest: true,
		rollupOptions: {
			output: {
				manualChunks: (id: string) => {
					if (id.includes("radix")) return "radix-ui";
					if (id.includes("sonner")) return "radix-ui";
					if (id.includes("lucide")) return "icons";
					if (id.includes("heroicons")) return "icons";
					if (id.includes("@tanstack")) return "tanstack";
					if (
						id.includes('react-router-dom') ||
						id.includes('@remix-run') ||
						id.includes('react-router')
					) {
						return '@react-router';
					}
					if (id.includes('node_modules')) {
						return 'vendor'; // Split vendor libraries
					}
					return null;
				},
			},
		},
	},

	server: {
		port: 3175,
		cors: true,
	},
});

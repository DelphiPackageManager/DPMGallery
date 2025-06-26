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
					if (id.includes('node_modules')) {
						return 'vendor';
					}

					return null;
					// if (
					// 	id.includes("lucide") ||
					// 	id.includes("heroicons")

					// ) {
					// 	return "icons"
					// }
					// // if (id === '\0commonjsHelpers.js') { return 'helpers'; }
					// // if (
					// // 	id.includes('react') ||
					// // 	id.includes('@remix-run')
					// // ) {
					// // 	return 'react';
					// // }
					// // if (
					// // 	id.includes("radix") ||
					// // 	id.includes("sonner")
					// // ) {
					// // 	return "radix-ui";
					// // }
					// //if (id.includes("@tanstack")) return "tanstack";
					// if (id.includes('node_modules')) {
					// 	return 'vendor'; // Split vendor libraries
					// }
					// return "unknown";
				},
			},

		},
	},

	server: {
		port: 3175,
		cors: true,
	},
});

/** @type {import('vite').UserConfig} */
export default {
    build: {
        outDir: '../wwwroot/dist',
        emptyOutDir: true,
        sourcemap: true,
        lib: {
            name: 'blog',
            entry: ['src/main.ts'],
            fileName: (format, entryName) => `${entryName}.${format}.js`,
            cssFileName: 'theme',
        }
    }
}
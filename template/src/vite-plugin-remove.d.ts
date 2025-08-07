declare module 'vite-plugin-remove' {
    const remove: (options: { targets: string[] }) => import('vite').Plugin
    export default remove
}

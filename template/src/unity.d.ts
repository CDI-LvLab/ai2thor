export {}

declare global {
    interface Window {
        createUnityInstance: (
            canvas: HTMLCanvasElement,
            config: object | null,
            onProgress?: (progress: number) => void,
        ) => Promise<any>
    }
}

export {}

declare global {
    interface Unity {
        SendMessage: (object: string, command: string, data: string | number) => void
    }
    interface Window {
        Unity: Unity | null
        lastEvent: object
        createUnityInstance: (
            canvas: HTMLCanvasElement,
            config: object | null,
            onProgress?: (progress: number) => void,
        ) => Promise<Unity>
        onUnityMetadata: (data: object) => void
        onGameLoaded: () => void
    }
}

export {}

declare global {
    interface Unity {
        SendMessage: (object: string, command: string, data: string | number) => void
    }
    interface Window {
        Unity: Unity | null
        ai2thor: {
            metadata: string
            images: { [id: string]: Uint8Array }
        }
        createUnityInstance: (
            canvas: HTMLCanvasElement,
            config: object | null,
            onProgress?: (progress: number) => void,
        ) => Promise<Unity>
        onUnityImage: (key: string, image: Uint8Array) => void
        onUnityMetadata: (data: string) => void
        onGameLoaded: () => void
    }
}

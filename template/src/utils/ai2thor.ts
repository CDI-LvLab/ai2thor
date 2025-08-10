export function setupUnity() {
    if (!window.createUnityInstance) {
        console.error(
            "Can't find createUnityInstance(). Is /Build/Unity.loader.js correctly loaded?",
        )
        return
    }
    const canvas: HTMLCanvasElement | null = document.querySelector('#unity-canvas')
    if (!canvas) {
        console.error('Could not get the canvas element for Unity (#unity-canvas)')
        return
    }
    window
        .createUnityInstance(canvas, {
            dataUrl: `Build/Unity.data`,
            frameworkUrl: 'Build/Unity.framework.js',
            codeUrl: 'Build/Unity.wasm',
            streamingAssetsUrl: 'StreamingAssets',
            companyName: 'CDI',
            productName: 'AI2THOR',
            productVersion: '1.0',
        })
        .then(async (unityInstance: Unity) => {
            console.log('Unity loaded!', unityInstance)
            window.Unity = unityInstance
            window.ai2thor = {
                metadata: '',
                images: {},
            }
        })
        .catch((message) => {
            console.error('Failed to load Unity:', message)
        })
}

export function initializeAi2Thor() {
    // Set controller to enable interaction
    window.Unity?.SendMessage('FPSController', 'SetController', 'HRI')
    // Initialize multi-agent
    window.Unity?.SendMessage(
        'FPSController',
        'Step',
        JSON.stringify({
            action: 'Initialize',
            gridSize: 0.25,
            agentCount: 2,
            fieldOfView: 65,
            renderImage: true,
            onlyEmitOnAction: true,
        }),
    )
}

export function command(action: string, args: object) {
    window.Unity?.SendMessage(
        'FPSController',
        'Step',
        JSON.stringify({
            action: action,
            agentId: 0, // Default to the human
            renderImage: true,
            onlyEmitOnAction: true,
            ...args,
        }),
    )
}

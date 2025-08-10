// helper to make a 4-byte big-endian length buffer
function u32BE(n: number) {
    const b = new Uint8Array(4)
    const dv = new DataView(b.buffer)
    dv.setUint32(0, n, false) // big-endian
    return b
}

export async function sendAi2ThorPacked(
    send: (data: Blob) => void,
    payload: {
        metadata: string
        images: { [id: string]: Uint8Array }
    },
) {
    // Build meta listing image ids and lengths in a deterministic order
    const ids = Object.keys(payload.images)
    const imagesArr = ids.map((id) => ({ id, byteLength: payload.images[id].byteLength }))
    const metaObj = { ai2thor: { metadata: payload.metadata, images: imagesArr } }
    const metaStr = JSON.stringify(metaObj)
    const metaBytes = new TextEncoder().encode(metaStr)
    const metaLenBuf = u32BE(metaBytes.byteLength)

    // Create Blob parts: [metaLen, metaBytes, ...image Uint8Arrays]
    const parts: (ArrayBuffer | ArrayBufferView | Blob)[] = [metaLenBuf, metaBytes]
    for (const id of ids) parts.push(payload.images[id])

    // Use Blob to avoid forcing a single large contiguous copy in many engines
    const blob = new Blob(parts)

    // send the blob — WebSocket.send accepts Blob, ArrayBuffer, Uint8Array
    send(blob)
}

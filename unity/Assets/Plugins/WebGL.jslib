mergeInto(LibraryManager.library, {
  Init: function () {
    // window.alert("Init test!");
    if (window.onGameLoaded && typeof window.onGameLoaded === "function") {
      window.onGameLoaded();
    }
  },

  SendMetadata: function (str) {
    if (
      window.onUnityMetadata &&
      typeof window.onUnityMetadata === "function"
    ) {
      window.onUnityMetadata(Pointer_stringify(str));
    }
  },

  // SendImage: function(key, pointer, length) {
  //   console.log(key, pointer, length);
  //   if (window.onUnityImage && typeof window.onUnityImage === "function") {
  //     var bytes = new Uint8Array(Module.HEAPU8.buffer, pointer, length);
  //     window.onUnityImage(Pointer_stringify(key), bytes);
  //   }
  // }

  SendImage: function (keyPtr, arrayPtr, length) {
    try {
      var key = UTF8ToString(keyPtr);
      var bytes = new Uint8Array(Module.HEAPU8.buffer, arrayPtr, length);
      if (window.onUnityImage && typeof window.onUnityImage === "function") {
        window.onUnityImage(key, bytes);
      } else {
        console.warn(
          "onUnityImage() handler missing! You won't receive image data."
        );
      }
    } catch (e) {
      console.error("Error processing image data.", e);
    }
  },

  SetCursorStyle: function (stylePtr) {
    var style = UTF8ToString(stylePtr);
    document.getElementById("unity-canvas").style.cursor = style;
  },

  AssetsLoaded: function (stylePtr) {
    if (window.onAssetsLoaded && typeof window.onAssetsLoaded === "function") {
      window.onAssetsLoaded();
    }
  },

  EmitEvent: function (eventStr, messageStr) {
    if (window.onUnityEvent && typeof window.onUnityEvent === "function") {
      window.onUnityEvent(
        Pointer_stringify(eventStr),
        Pointer_stringify(messageStr)
      );
    }
  },
});

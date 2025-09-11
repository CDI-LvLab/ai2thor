using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Thor.Procedural;
using Thor.Procedural.Data;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class RemoteAssetManager : MonoBehaviour {

    public string AssetBase = "http://localhost:5173/objects/models";
    public string[] AssetIds = {
        "0a3db3f1bec64abbb2b2c0950e0ba6df",
        "0a4e5f28b320403992b87fdaf5bf27b2",
        "0a6b0e23465f4f2898194619edafb523",
        "0a20b303d3714aebb08517df7b8747ad",
        "00a57bbd1c894a7cb9e45ec396a8f660",
        "0abdb5517f7843fbacb488468cfff1b7",
        "0acda59caa0b4a928cec65bb668e08c1",
        "0ad11df941714f148127620056ed7c74",
        "0b5dc5d4f24c4ae2b0746d855af5c423",
        "0b667a9ce3aa445bbc8118fd6f6356f8",
        "0b29453835364f39858de181ee5e891f",
        "0ba043f8a6ee4419895e15969beec3c4",
        "0bb52ec746cf4fc5b4448c52bff00aea",
        "0bd252bdafe146c2bf5bc9aed43b623d",
        "0bde272bb07b4e1a99fd2f981926e187",
        "0c8efdf9b21a41ad9e25d7cebf172e0f",
        "0c8fe88fbb324b278bf4bb3742bb38b6",
        "0c12f785bfb24911ba64741a80d3bbdf",
        "0c39d151636240fcbcc23173e1ba5221",
        "00c86cb620a743cc81d9cb87ad4d8696",
        "0c84252f66b44ed5a1b48f65bc11b43c",
        "0ce9d8a62f2641b8af62a8f542f94812",
        "0d9defb2bbb04b35b44ac438f3fedb7b",
        "0d41c125cab841ddb05502ee68350630",
        "0d79bad60595405c8690a4e9cdde47cb",
        "0d1971dfade24524aa4c3307b4104def",
        "0dc6774e55e14203a4bf50dc3254b64c",
        "0dd2a337078e4f72896c10b533dbf5a0",
        "0dd8be84d117499d9adfae347dc5a389",
        "0dd49bcf03894c208e3e6f9a17909f9a",
        "0e0b73b1eee34daa91757142d3260c94",
        "0e9b6e96a047410a80c31adddddb2e11",
        "0e485c2b8eb745e190c71a3e32fe4741",
        "0e793eb207df4d2ba8925e4d0d11e6bb",
        "0f2ba5e3a05246f48abe54f87954e034",
        "0f995f2ee1f4489182eb81aae5731366",
        "0fb0e3138d1b475c838a0fcf84011736",
        "0ff05732c5794cea8a5b818c731de2f5",
        "1a723a9d046e4ba0a539d35050f6f60a",
        "1a7662abb0394961a9e9b08aad684690",
        "1ab9bf841df04c07b1819be596327629",
        "1af8d7d21008465b98e6e619de0a8540",
        "1b99b250bced4666b709b6d911f1f152",
        "1bc8c225d72947109688d87aee021e01",
        "1bf1842e51264add93c9d986919e3ed7",
        "1bfe60faa66d44e4ad922e64e39e3ab5",
        "1c0bedab21bc441fb846f6a9003f020e",
        "1c87b41249f14e5bb73b62c08633209f",
        "1c544dfbf2344889bb7b91edbf71502a",
        "1c9866a995084b7ba237b8bcd36f68a2",
        "01cebe814ca34b60867fe50d44ee023a",
        "1d0c965ea0124bf0acf426de59034364",
        "01d5fbe1cfb748569ae3c2ebf1f1abe8",
        "01d7fd4af8f04aa7b7e33eddfaf10b42",
        "1d8a0711dd974f729ab76d2af7daa529",
        "1d41b94ffb8f4e33a70349545c9cbdc6",
        "1d471a63a6b0407e8fb6b8ba76cdfddb",
        "1df6c11f0c6e43249473150685984000",
        "1ed85d7b6ccc447396ec14997cae1880",
        "1ee896f728d94bcd9937476eff693be2",
        "1ee1268a508c40b28b1e7c734ba583ec",
        "1f9f025d995c4554bcc39b97eed20c7d",
        "1f33e40c27fe451e9efda612c69010ae",
        "1f2954e0f009428995968912ea1602f5",
        "1facc22c744a403dbb3166a2274f78dd",
        "2a37a4b61b8c4aad9e4eb78a689913a4",
        "2a051dfbdcc040ffa0db1e4b8c20c93a",
        "2bd76952a7a24ca7ac7bd5d7940de959",
        "2be81dcbd5824aa6a5c6f596ca7c9a63",
        "2c2c9fda7cec440aa1362e9c6173d34e",
        "02c4ee90724144fba9cb896602318f2d",
        "2d89450e98bf43d58168096b16a42abe",
        "2d15111667414c82b83b4e37b263bad3",
        "2da63c1b8ac247dd8b3248fbc86a6232",
        "2df5a685ee5e410c8b814a42376faf01",
        "3a924ceaef0c43d9ad37db9feb2eddb6",
        "3afcdd4eb1654f31839a672377cfdd8d",
        "03b0ac7d99c44197a09640179f360f3c",
        "03bb0fbd0fc046f6aaf4b6f6185176ab",
        "3bb9af9375564dc786edb0c72b56dd2e",
        "3ce7e90784d448c4bfc7491c49e53cfb",
        "3d8d5ea391bd4f30bb0603eb07f17ca9",
        "3e6864109fcd4261951e47a8297b3d6a",
        "3eee804c886e492c99bb746fac2557ef",
        "3ff309d1ff3e4b43a7e5fd3d392f3962",
        "04a8b22011c8466dada8deca1fd56ae3",
        "4af5b7533cef44e599d00e7063d6de63",
        "4b27628be0804c0285d0b13dda025a0d",
        "4cbfd7400aa6477daa518b79712f1c43",
        "4d490b545571442898d63c27719c8779",
        "4d7261585ce34041af9603f9177ccbad",
        "4e15375f0ea04d80afb22229cda1b9de",
        "05a12bc9a23c4c7bb5a84601b61221ce",
        "5a83a0f4018542d1afd69725ae9d80c8",
        "5a5467c967144693a540a085f51818c3",
        "5bc59f7f9eb1426781a1dcbd9f08efe5",
        "5bdbe3247b3f4c4a8d97ff0ff51f711d",
        "5cc2d665f9114714bbb2a801ff86766a",
        "5d78e071918a49b49c74b64d957b4a03",
        "5df52b78e98c4885bf45a4c6fd76cfd6",
        "6ac5937fd2594729ae98ad71af3e33d2",
        "6c8970b21bf540cdb3616d885c4c7821",
        "6d03fe19da944b948695dfbc7060f031",
        "6db831285d1d447997b9b9bc06a0f503",
        "6e405f0169d9449a852bfde88d12e3ed",
        "6f1ac304029143b79f038536d79480e6",
        "7aa9002b63e9405e81d9985790acde36",
        "7aef0ed371da410cbb56f1188c83a5aa",
        "007b3198be5741218750bd836559047a",
        "7bfa596d1cfe44d298243636a0b611fc",
        "7c3e6bcb60ed47ffa91bf823c68f36b9",
        "7c123230a9244b6bbcfb51b63a1194de",
        "7cdf0ab3d55242858dc9e728501a4c72",
        "7e5d8eb8aa2943e19d067239bec39618",
        "8ab5ad93be4942758c4c3ec4c8bb3e5f",
        "8c51d37795264cc99961417a56b3c1c0",
        "8c395abf85934356980e718fc1094736",
        "8d6d039c69924b31bb7792e730dde0cd",
        "8e463518e78f4f4ab95fd5826532900d",
        "8ebd4e7d813a44158116f5f3a6ab4ce8",
        "8f119c3e36f1484aaa9bec431bea14b4",
        "8fd5608b449f4cde81a8d924cdaed416",
        "9b4f65ed3ba54bce84b5508392c3e8aa",
        "9b60afd0057d44bf9230abb3816376f7",
        "9c4b86c8c22844f1b968a2d8875fce8d",
        "9c8483481a134ecf84d3864b45faca6a",
        "9cacb45857a14e5b8f9fb4094f421cc2",
        "9d24aac8a4c444a1b440647aa6f96438",
        "9ddb71ab0419472892fe5cf8a87917f3",
        "9e8cc8ba31504719abc726baf9069a0b",
        "11d4fd6435cc4375b8f6427421ca7bab",
        "12fc3734ede4419eaf0092c035f7b05c",
        "13dbe30b0e45408c8bfaddfe6a4e8786",
        "14a4c834f401471c8ba59685d799f24c",
        "14a4745557e24661bbc1515ed3882dae",
        "14d3d23449264408b748a26b6ce7a587",
        "018dab01b31949318128504332f649d9",
        "19e9ec41b4234e46a3503e8376f3e7a5",
        "20b26cd4356a42f28a3550b7eff7cc1d",
        "21d8e6f8fdbe461c92a20f6360654d4e",
        "21ddf470a3fe4095a0ef58778e897187",
        "23b2d0e81d414836877c555e5b5fc18d",
        "24d3d4de60a14e748c97017a21c7469e",
        "26da76a27d254fa89c63d0b03267471c",
        "26f81136570e41ec96120c3a4f70fcba",
        "27eaeb00a8ee4dc2a43047a74aeb4770",
        "30d6fefb2da84469ae2ce27bcf8fa6fc",
        "34ed997a8ccc4decab05c3d4b67652c8",
        "37d74e119c8248abb162c2887ecfbc79",
        "40b28d1cbfbd4e9f9acf653b748324ee",
        "42e1e3a4e80443b8ac72fd55d988153e",
        "45bccd8afa834e84b690fb91e34f9262",
        "0047fa5e44334d3e9bd171ad2bf95ff5",
        "49b2bb2c0f8048859956fd02ca43525d",
        "49fd7085feae450b8d3676fd8f1499b9",
        "50e31d461023475796a281239d7a0c0c",
        "52f142c052564d85b6ef21413af34e25",
        "57c18917321f4c2aa762c901b7ad7e8d",
        "59e20f0d798e44a481da90f0afe33c9c",
        "60ada468b65c4058aa1db3dbfcd7e439",
        "60d3cb0210bf4c2e9c802cf35122116f",
        "64fdc917a7934b07a1ac3287e9ade405",
        "65c0171c84ac41bb84dcefd773ff11b2",
        "66f30dd545df487abf2405575de0542d",
        "067a53c4821047888aea55eadbafb3df",
        "71ad959a42fb43fc9b819fdc4368f417",
        "71be513838c048669b97f187f5347419",
        "75c5052910ea46feaad64509d96556fe",
        "76c11a5e35b942c7ab707aa84c5cacc7",
        "77a8cd053a0a4873a4b075bfcf39885e",
        "81b129ecf5724f63b5b044702a3140c2",
        "81c0859214e847fba2421161ffc1a24c",
        "081d7d38825047c6971d4faa15a36183",
        "85b8c65d7c90485e85ac7cd5658198da",
        "88d568cc95d94c2dbda90b39db8affbf",
        "90b588cf07a94933b8fbb61c8f889104",
        "090cf42c4c1441baae4ba6836a7367de",
        "92e21f2fc6b240ca8845209693099cb2",
        "95dccc6375144c0cac24c4cabbab375d",
        "98acd80e57204118ba67acf93b3ad735",
        "99df8b2ce6c14143b598fb5ff7b914e5",
        "127e434667c54313b9a9300404f7f8bd",
        "127ed080abe44df1a34c17e5f88f3af5",
        "142cd0af58a2445f9d42c266cffee72f",
        "173d41bbb6554e1cb623160f8e73013b",
        "212d33f6b5904bed965a87b018325b26",
        "215fd2f1bec3457fbf2fd5fa36c806c8",
        "243a539d3c1640498536f3d60d3d884b",
        "319afc33920e4fc7b3ba3f709157966e",
        "368f525cf8704607bc37f6eacbdc48d8",
        "0403fa716fe748ce9120e267b97fa6fe",
        "604dcd7c59cd42efafa2e5a0a7bd142d",
        "642bc90f84e5480dab113e3b9376ff32",
        "721bb93d169a4d5f9e8dd15ad45a367f",
        "0758a7c254824c2684f7df3b8550bcff",
        "791bff1f2c72441dad8db59677f2e036",
        "848b2469b1cb49818b3fc6cca8b9b398",
        "890f337cb0944c98957a8e9fac7be77b",
        "926ff028e22a4a628b76baba18e8d94e",
        "1669f0e8e23d4160ae064c39867f6218",
        "04039fa35c1749c5bc4cc23dee338af4",
        "05308bf4da874a63b040399dca39489b",
        "8122f93811f94a42bb653823860502da",
        "8208e17164a04672bfabc6152ff43e71",
        "8521f9252bb94938aee3af96715663b3",
        "8573cd82db1646c6b637602997e8a949",
        "9175fe834fde4f408e958bf209cae9c9",
        "9645cd0a8e084a60bc86fbfff7862c34",
        "9676e66c704c4927898c7e31e4ee0aaf",
        "9681d78524fd427daecb14403c320c8c",
        "13953d0dc7ad44568595ae82e751eaed",
        "16416c467b174bc3b487655e95318987",
        "36889c08436941d087b9ffeb9922f1d9",
        "42868e54df564f21a1cf9413c8130e22",
        "51010c5d6a0f4689b14cd7e999616ce3",
        "057934f97c4042e09a9882543dd5341e",
        "068525be5e4e44aaa10d34c01f72f545",
        "92001acb725e40f79a62ec4cd278d845",
        "95198e3460b14c4db3749eb888a869b3",
        "119343ee162a44bfa49f53b2686b2fb9",
        "141202eed32747b7b1259a3efeba9c96",
        "340298f1355148c19eae099ca49e033b",
        "399869c53f7940878e3c106314e5e440",
        "585855de381a44ba9860c3d3887b2f8b",
        "619563fb2e33466cb1286b5732da59f9",
        "785001de38b04c7887ff981592aa9462",
        "849699a8b183425eba6a6a0924e01429",
        "947753cea4e64737a3b7023e216a70d9",
        "2016446c3bf34c27ba35df40827f1ded",
        "9499783bed2d44b99112ec18cb5259d4",
        "10855437b8b6486a8020e25f74820a26",
        "63966903fe8344db81d707ccc3fbac4d",
        "639955874f824bc381702faf7684d779",
        "5124019656c140b7924269dba379a808",
        "20235580147f40b19faec9de5bb64639",
        "0079418517784f2bbf0aec6fd8de3cde",
        "281538859489450cb0ba1be1d0275488",
        "a3db17aa31074ce490e288cc97d9fad2",
        "a4f6137d1246407f95adff5ea3ed6ed0",
        "a320de07f19e41e6a18737be955177fb",
        "a670a16c94e941158541d959a5960891",
        "a784c3ce29d44a899afe3f80a20e4fec",
        "a5483e74a73143cdb3a453ebd167ce06",
        "a166087409db4c59b3d35fceff17a8c5",
        "aec6041c449a446697c14a67a7047f1a",
        "b920d0f5eb53414e966a04b6ec525fcf",
        "b82767773b92447b85607c73661c7576",
        "ba85570abe2d4f738df7b7f2702844b6",
        "bafce5277aa84954935da388ac423dda",
        "bb62112ffc3a4a62af773f56978f52fb",
        "bc73c5ee963f474186f6409f93f6dcd8",
        "bc2523db62254235a46ac9c13cff48ee",
        "bd9899bf841a4c1c918e01d7cde41211",
        "c2ca972ad18045fd85849414d24c5813",
        "c4c8ee02e74b4a1a825da6daedfb6a4c",
        "c39da302d09f42ada448dbf26663c768",
        "c59a1819d5024611a2deae8c75fda74e",
        "c202f0c8f0d64d8d847c54776955089c",
        "c04952f037034aa2836da967f66a8fe4",
        "c79865c2b8bb414d8b88bd599e72eb42",
        "c673051f863143a481c399f3986d66f9",
        "c18644406e1a434688d13d84d7356022",
        "cba4bcda468540c7a3ff843b08f22412",
        "cbc5941a5476410598c553e619b70bc9",
        "cd4dba634164441f850f42933885ea71",
        "cdeadc5a6e3b4377acdd5c158c385163",
        "ce27e3369587476c85a436905f1e5c00",
        "ceb9ee9947b840bb97462f56eb434e64",
        "cee4802cafe3483c9d72f27d6f3e06d1",
        "cf550abe593043c7b3fdc839863073e1",
        "d2d7c05e2c844fc2bc0d7519de935be9",
        "d5c3f159bccb41a8b5d18314bc1d099f",
        "d5d3ec0179ef4b41b5977f3ae1f40fb7",
        "d7de06b2b4ff4eb18b7d5f5d9c3ae6e2",
        "d7e15badbd234036ae12bd1294256094",
        "d9ffbc4bbe1044bb902e1dddac52b0de",
        "d76a2b77e7074339937fa4e750d10002",
        "d082daab83394502976f5e156c9fe6a5",
        "d6594cf163044fce92e4e8f539257391",
        "d6849de2f338430faf6fa872168af215",
        "d62583f82dfe46f0a8a8255d35a6de62",
        "dae6c93b966c484795e076cdab039631",
        "dafa3afce8bc4ba5a8d31a0240259bf2",
        "dbab301d6a2647cf8b7b3d50c4cec3d7",
        "e0b20bdfef7b468395cd4abeffd5edda",
        "e0d5ec7eb4c842599205c6f65b14bdd3",
        "e1a4187f2ac24ae384a244c0b8370170",
        "e2a715e676134f2fb31f005dcea5b227",
        "e2e86e27014c4aa7a1fa708db44cebda",
        "e09c5d06626d4b19aa0aeab0d71e5479",
        "e60dca44482045ce90a71946888a53b1",
        "e69dc287b3994ff0923f83bb47eaa374",
        "e93e7dbbe1724a309b725d5acc254334",
        "e1123c5e510d4a418a6efd3c9cd18953",
        "e074099f0ce948acb1c7b875fec45edf",
        "eb4a20320c1e40f7b5e58f89edd46942",
        "ec7db4ed503a42edb31aa8184db6094c",
        "ede795f1c6f74a99b6952183f340fcb8",
        "ee3043c821ef41f9a7e32826af267d20",
        "eeae8668106246398fc7956d2fb225e5",
        "ef4d2daeecff4742b6b3bd92190f0adf",
        "f1b0c6ffa28d4dcc8df653627dfc2b27",
        "f1c26b1d151f4c9680022a68f889d2e4",
        "f4b87f0a9b56429d97488e2bb5bc9cd4",
        "f08d4519cf2043748dc9f92ca30658ef",
        "f44ca5eb44fe444d8ad1890cd5cfccf0",
        "f56ceb6d1b6f413b84e7602c574640f2",
        "f56ebfe229ae404080d24496c9ee4f3d",
        "f74bfe01ca1e459b8c730a8881ea88bb",
        "f506b4a0cc9346f9acd9d7fff508382e",
        "f0916a69ba514ecc973b44528b9dcc43",
        "f3417a0837f048a1830f3b52171bcb04",
        "f5412f0a56ad495ca3a448e59d5b1ab8",
        "f8985c2ac7c644b096dff60d816b869b",
        "f24611d4c265453ba4220085a8d1b0a4",
        "f331379db90a4b73bf09ff4bc74889ee",
        "f471783cda56422793cb5d65e1041ff3",
        "fabd9b92e1b74467aa5120f52e8c32ab",
        "fb19ffdaa20143209ad2aa35ddfbaed1",
        "fc7d291383a94b48847a71fe9f3fe1ac",
        "fc55a7410d674754a8963216cf728f80",
        "fd3a41ef166a4bdd916c3f8c5ffc458e",
        "fd058916fb8843c09b4a2e05e3ee894e",
        "ff79f84eef294f46a68cb86836a9efb3",
        "ffd3f7044de548fa901dff07f77271a8"
    };

    [DllImport("__Internal")]
    private static extern void AssetsLoaded();

    //     async void Start() {
    //         try {
    //             var remaining = new HashSet<string>(AssetIds);
    //             int retries = 3;
    //             float delaySeconds = 0.1f;
    //             int batchSize = 5; // smaller batch for smoother background loading

    //             for (int attempt = 1; attempt <= retries && remaining.Count > 0; attempt++) {
    //                 Debug.Log($"[RemoteAssetManager] Attempt {attempt}/{retries} for {remaining.Count} assets.");

    //                 var currentBatch = new List<string>(remaining);

    //                 for (int i = 0; i < currentBatch.Count; i += batchSize) {
    //                     var batch = currentBatch.Skip(i).Take(batchSize).ToList();

    //                     foreach (var assetId in batch) {
    //                         var result = await CreateRemoteRuntimeAsset(assetId);

    //                         if (result.success) {
    //                             Debug.Log($"✅ Loaded asset '{assetId}'");
    //                             remaining.Remove(assetId);
    //                         } else {
    //                             Debug.LogWarning($"❌ Failed asset '{assetId}' attempt {attempt}: {result.errorMessage}");
    //                         }

    //                         // Yield control back to the browser so UI remains responsive
    //                         await Task.Yield();
    //                     }
    //                 }

    //                 if (remaining.Count > 0 && attempt < retries) {
    //                     Debug.Log($"Waiting {delaySeconds * attempt} seconds before retrying {remaining.Count} assets...");
    //                     await Task.Delay(TimeSpan.FromSeconds(delaySeconds * attempt));
    //                 }
    //             }

    //             if (remaining.Count > 0) {
    //                 Debug.LogError($"Failed to load {remaining.Count} assets after {retries} attempts: {string.Join(", ", remaining)}");
    //             } else {
    //                 Debug.Log("All remote assets loaded successfully!");
    // #if !UNITY_EDITOR && UNITY_WEBGL
    //                 AssetsLoaded();
    // #endif
    //             }
    //         } catch (Exception ex) {
    //             Debug.LogError("Fatal error in Start: " + ex);
    //         }
    //     }



    /// <summary>
    /// Creates a runtime ProceduralAsset from a .json.gz URL asynchronously.
    /// </summary>
    public async Task<ActionFinished> CreateRemoteRuntimeAsset(
        string id,
        ObjectAnnotations annotations = null,
        bool serializable = false
    ) {
        string url = string.Format("{0}/{1}/{1}.json", AssetBase, id);
        var assetDb = FindObjectOfType<ProceduralAssetDatabase>();
        if (assetDb.ContainsAssetKey(id)) {
            return new ActionFinished(
                success: false,
                errorMessage: $"'{id}' already exists in ProceduralAssetDatabase. Use SpawnAsset instead.",
                toEmitState: true
            );
        }

        ProceduralAsset procAsset;
        try {
            procAsset = await LoadAssetFromUrlAsync(url, annotations, serializable);
        } catch (Exception ex) {
            return new ActionFinished(
                success: false,
                errorMessage: $"Failed to create asset from '{url}': {ex.Message}",
                toEmitState: true
            );
        }

        procAsset.serializable = serializable;

        return await CreateRuntimeAsset(procAsset, AssetBase, returnObject: true);
    }


    /// <summary>
    /// Downloads and loads a ProceduralAsset from a .json.gz URL
    /// </summary>
    public static async Task<ProceduralAsset> LoadAssetFromUrlAsync(
        string url,
        ObjectAnnotations annotations = null,
        bool serializable = false
    ) {
        if (!url.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) {
            throw new ArgumentException("Only .json assets are supported.");
        }

        // Step 1: Download asset file
        using UnityWebRequest www = UnityWebRequest.Get(url);
        var op = www.SendWebRequest();
        while (!op.isDone) {
            await Task.Yield(); // prevent blocking main thread
        }

        if (www.result != UnityWebRequest.Result.Success) {
            throw new IOException($"Failed to download asset: {www.error}");
        }

        // Step 2: Parse JSON directly (browser has already decompressed it)
        string json = www.downloadHandler.text;

        // Step 3: Deserialize JSON
        var jsonResolver = new ShouldSerializeContractResolver();
        var serializer = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = jsonResolver,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        var procAsset = JsonConvert.DeserializeObject<ProceduralAsset>(json, serializer);
        procAsset.serializable = serializable;
        procAsset.annotations = procAsset.annotations ?? annotations;

        return procAsset;
    }


    private static async Task<Texture2D> LoadTextureFromUrlAsync(string url) {
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        var op = www.SendWebRequest();
        while (!op.isDone) {
            await Task.Yield();
        }

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError($"Failed to load texture '{url}': {www.error}");
            return null;
        }

        return DownloadHandlerTexture.GetContent(www);
    }

    public static async Task<ActionFinished> CreateRuntimeAsset(ProceduralAsset asset, string urlBase, bool returnObject = false) {
        var assetDb = FindObjectOfType<ProceduralAssetDatabase>();
        if (assetDb.ContainsAssetKey(asset.name)) {
            return new ActionFinished(
                success: false,
                errorMessage: $"'{asset.name}' already exists in ProceduralAssetDatabase, trying to create procedural object twice, call `SpawnAsset` instead.",
                toEmitState: true
            );
        }
        Texture2D albedoTexture = null;
        Texture2D metallicSmoothnessTexture = null;
        Texture2D normalTexture = null;
        Texture2D emissionTexture = null;
        if (asset.albedoTexturePath != null) {
            string url = string.Format("{0}/{1}/{2}", urlBase, asset.name, asset.albedoTexturePath);
            albedoTexture = await LoadTextureFromUrlAsync(url);
        }
        if (asset.metallicSmoothnessTexturePath != null) {
            string url = string.Format("{0}/{1}/{2}", urlBase, asset.name, asset.albedoTexturePath);
            metallicSmoothnessTexture = await LoadTextureFromUrlAsync(url);
        }
        if (asset.normalTexturePath != null) {
            string url = string.Format("{0}/{1}/{2}", urlBase, asset.name, asset.normalTexturePath);
            normalTexture = await LoadTextureFromUrlAsync(url);
        }
        if (asset.emissionTexturePath != null) {
            string url = string.Format("{0}/{1}/{2}", urlBase, asset.name, asset.emissionTexturePath);
            emissionTexture = await LoadTextureFromUrlAsync(url);
        }
        ObjectAnnotations annotations = asset.annotations;
        if (annotations == null) {
            annotations = new ObjectAnnotations();
            annotations.objectType = "Objaverse";
            annotations.primaryProperty = "CanPickup";
        }

        var assetData = ProceduralTools.CreateAsset(
            vertices: asset.vertices,
            normals: asset.normals,
            name: asset.name,
            triangles: asset.triangles,
            uvs: asset.uvs,
            albedoTexture: albedoTexture,
            metallicSmoothnessTexture: metallicSmoothnessTexture,
            normalTexture: normalTexture,
            emissionTexture: emissionTexture,
            colliders: asset.colliders,
            physicalProperties: asset.physicalProperties,
            visibilityPoints: asset.visibilityPoints,
            annotations: annotations,
            receptacleCandidate: asset.receptacleCandidate,
            yRotOffset: asset.yRotOffset,
            returnObject: returnObject,
            serializable: asset.serializable,
            parentTexturesDir: asset.parentTexturesDir
        );
        return new ActionFinished { success = true, actionReturn = assetData };
    }
}

using MelonLoader;
using UnityEngine;
using Il2CppSystem.Collections.Immutable;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;
using System.Collections;

using VRC.SDKBase;

namespace UdonSpy
{
    public class UdonSpyClass : MelonMod
    {
        GameObject UdonSpy1;
        Text text;
        bool active;
        object routine;

        bool Active
        {
            set
            {
                if (UdonSpy1 != null)
                {
                    active = value;
                    UdonSpy1.SetActive(active);
                    //text.text = "";
                }
            }
            get
            {
                return active;
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == 1)
            {
                if (VRCUiManager.field_Private_Static_VRCUiManager_0 != null)
                {
                    LoggerInstance.Msg("VRCUiManager Instantiated");
                    OnVRCUiManagerInstantiate();
                }
            }

        }

        void OnVRCUiManagerInstantiate()
        {
            if (routine == null)
                routine = MelonCoroutines.Start(WaittingForUIInstantiated());
        }

        IEnumerator WaittingForUIInstantiated()
        {
            float count = 0;
            while (GameObject.Find("UserInterface") == null || GameObject.Find("UserInterface").transform.Find("MenuContent/Popups/InputPopup") == null)
            {
                yield return null;
                count += Time.deltaTime;
                if (count > 10)
                {
                    LoggerInstance.Error("Initialization Failed");
                    break;
                }
            }
            if (count <= 10)
            {
                LoggerInstance.Msg("Start to Instantiate UdonSpyInputField");

                UdonSpy1 = Object.Instantiate(GameObject.Find("UserInterface").transform.Find("MenuContent/Popups/InputPopup").gameObject, GameObject.Find("UserInterface").transform.Find("MenuContent/Popups/InputPopup").parent);
                UdonSpy1.name = "UdonSpyInputField";
                UdonSpy1.SetActive(false);
                Object.Destroy(UdonSpy1.GetComponent<CanvasGroup>());
                Object.Destroy(UdonSpy1.GetComponent<VRCUiPopupInput>());
                Object.DestroyImmediate(UdonSpy1.transform.Find("Darkness").gameObject);
                Object.DestroyImmediate(UdonSpy1.transform.Find("CharactersRemainingText").gameObject);
                Object.DestroyImmediate(UdonSpy1.transform.Find("ButtonLeft").gameObject);
                Object.DestroyImmediate(UdonSpy1.transform.Find("ButtonRight").gameObject);
                Object.DestroyImmediate(UdonSpy1.transform.Find("PasswordVisibilityToggle").gameObject);
                Object.DestroyImmediate(UdonSpy1.transform.Find("Keyboard").gameObject);

                UdonSpy1.transform.Find("TitleText").GetComponent<Text>().text = "GameObject Path";
                text = UdonSpy1.transform.Find("InputField/Text").GetComponent<Text>();

                UdonSpy1.transform.Find("ButtonCenter").GetComponent<Button>().onClick.RemoveAllListeners();
                System.Action action = () => GetProgramProperty(text.text);
                UdonSpy1.transform.Find("ButtonCenter").GetComponent<Button>().onClick.AddListener(action);
                UdonSpy1.transform.Find("ButtonCenter/Text").GetComponent<Text>().text = "Find";


                LoggerInstance.Msg("UdonSpyInputField Instantiated");
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                Active = !Active;
            }
        }


        private void GetProgramProperty(string goName)
        {
            if (goName == null || goName == "")
            {
                LoggerInstance.Warning("Name==null||Name==\"\"");
                return;
            }

            if (goName.StartsWith(@"-gpv "))
            {
                string[] strArray = goName.Split(new string[] { @" -nyh ", @"-gpv " }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in strArray)
                {
                    LoggerInstance.Msg(str);
                }
                if (strArray.Length != 2)
                {
                    LoggerInstance.Warning("Format Error,Please Use: \'-gpv GameObjectName -nyh VariableName");
                    return;
                }
                else
                {
                    if (GameObject.Find(strArray[0]) == null)
                    {
                        LoggerInstance.Warning("No Mathcing GameObject Found,Input Value:" + strArray[0]);
                        return;
                    }
                    if (GameObject.Find(strArray[0]).GetComponent<UdonBehaviour>() == null)
                    {
                        LoggerInstance.Warning("Failed To Get UdonBehaviour From This GameObject:" + strArray[0]);
                        return;
                    }
                    UdonBehaviour ub = GameObject.Find(strArray[0]).GetComponent<UdonBehaviour>();
                    if (ub.TryGetProgramVariable(strArray[1], out var value))
                    {
                        LoggerInstance.Msg(System.ConsoleColor.Blue, value);
                        LoggerInstance.Msg(System.ConsoleColor.Blue, value.ToString());
                        var a= value.TryCast<UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>>();
                        if(a!=null)
                        {
                            LoggerInstance.Msg(System.ConsoleColor.Blue, "Variable is an array");
                            foreach (var i in a)
                            {
                                LoggerInstance.Msg(i.ToString());
                            }
                        }
                      
             
                    }
                    else
                    {
                        LoggerInstance.Warning("Failed To Get Variable From Program");
                    }
                }
            }
            else
            {
                bool findAll = false;
                if (goName.EndsWith(@" -all"))
                {
                    string[] strArray = goName.Split(new string[] { @" -all" }, System.StringSplitOptions.RemoveEmptyEntries);
                    goName = strArray[0];
                    findAll = true;
                }
                if (GameObject.Find(goName) == null)
                {
                    LoggerInstance.Warning("No Mathcing GameObject Found,Input Value:" + goName);
                    return;
                }
                if (GameObject.Find(goName).GetComponent<UdonBehaviour>() == null)
                {
                    LoggerInstance.Warning("Failed To Get UdonBehaviour From This GameObject:" + goName);
                    return;
                }
                UdonBehaviour ub = GameObject.Find(goName).GetComponent<UdonBehaviour>();
                IUdonProgram _program = ub._program;
                IUdonHeap Heap = _program.Heap;
                IUdonSymbolTable SymbolTable = _program.SymbolTable;
                ImmutableArray<string> name = SymbolTable.GetExportedSymbols();

                if (name.Length != 0)
                {
                    LoggerInstance.Msg(System.ConsoleColor.Green, "-----------------------------------");
                    if (findAll)
                        name = SymbolTable.GetSymbols();
                    foreach (var s in name)
                    {
                        if (findAll)
                        {
                            var value = Heap.GetHeapVariable(SymbolTable.GetAddressFromSymbol(s));
                            if (value == null)
                            {
                                value = "null";
                            }
                            LoggerInstance.Msg("Key: " + s + "     Variable(Type): " + value.ToString() + "    " + value);
                        }
                        else
                            LoggerInstance.Msg("Key: " + s + "    Variable(Type): " + ub.GetProgramVariable(s).ToString() + "    " + ub.GetProgramVariable(s));
                    }
                    LoggerInstance.Msg(System.ConsoleColor.Green, "-----------------------------------");
                }
                else
                {
                    LoggerInstance.Msg(System.ConsoleColor.Green, "_exportedSymbols.Length=0,Try To Get All \'symbols\'....");
                    name = SymbolTable.GetSymbols();
                    if (name.Length == 0)
                    {
                        LoggerInstance.Msg(System.ConsoleColor.Green, "_symbols.Length=0");
                    }
                    else
                    {
                        LoggerInstance.Msg(System.ConsoleColor.Green, "-----------------------------------");
                        foreach (var s in name)
                        {
                            var value = Heap.GetHeapVariable(SymbolTable.GetAddressFromSymbol(s));
                            if (value == null)
                            {
                                value = "null";
                            }
                            LoggerInstance.Msg("Key: " + s + "     Variable(Type): " + value.ToString() + "    " + value);
                        }
                        LoggerInstance.Msg(System.ConsoleColor.Green, "-----------------------------------");
                    }

                }
            }

        }


    }
}

def CreateGenericClass( classname ):
    print("Creating " + classname + ".cs...");
    f = open("Assets/Scripts/Triggers/" + classname + ".cs", "w");
    f.write("using UnityEngine;\n\n");
    f.write("[RequireComponent(typeof(Collider2D))]\n");
    f.write("public class " + classname + " : MonoBehaviour\n{\n");
    return f;

def CreateBoolSwitchTrigger( vName ): 
    classname = "TrigSwitch" + vName[1:];
    f = CreateGenericClass( classname)
    f.write("\tvoid OnTriggerEnter2D(Collider2D _coll)\n");
    f.write("\t{\n")
    f.write("\t\tif(_coll.tag == \"Player\")\n");
    f.write("\t\t\t_coll.gameObject.GetComponent<Mario>()." + vName + " = !_coll.gameObject.GetComponent<Mario>()." + vName + ";\n");
    f.write("\t}\n}\n");
    f.close();
    
def CreateSetTrigger( vName, value, type ):
    if(type == "bool"):
        classname = "TrigSwitch" + vName[1:];
    else:
        classname = "TrigSet" +  vName[:1].upper() + vName[1:];
        
    f = CreateGenericClass( classname );
    f.write(type + " " + vName + " = " + value + ";\n\n\t");
    f.write("void OnTriggerEnter2D(Collider2D _coll)\n");
    f.write("\t{\n");
    f.write("\t\tif(_coll.tag == \"Player\")\n");
    f.write("\t\t\t_coll.gameObject.GetComponent<Mario>()." + vName + " = " + vName + ";\n");
    f.write("\t}\n}\n");
    f.close();
    
    
f = open("Assets/Scripts/Mario.cs", "r");

flag = False;
types = [];
names = [];
values = [];
for line in f:
    if not flag and not "// Start Variable Block" in line:
        continue;
    if not flag and "// End Variable Block" in line:
        break;
    
    if not flag:
        flag = True;
        continue;
    
    if "public" in line and not "()" in line:
        tmp = line.split("=")[0].replace("public ", "").strip().split(" ");
        types.append(tmp[0]);
        names.append(tmp[1]);
        values.append(line.split("=")[1].strip()[:-1]);
    
f.close();

for i in range(0, len(types)):
    CreateSetTrigger(names[i], values[i], types[i]);

    if types[i] == "bool":
        CreateBoolSwitchTrigger(names[i]);
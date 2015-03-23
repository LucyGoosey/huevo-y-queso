import os;

def CreateGenericEvent( classname ):
    classname += "Event";
    print("Creating event " + classname + ".cs...");
    
    f = open("Assets/Scripts/Triggers/Events/TEST/" + classname + ".cs", "w");
    
    f.write("using UnityEngine;\n\n");
    f.write("public class " + classname + " : TriggerEvent\n{\n\t");
    
    return f;

def CreateEvent( name, value, type ): 
    f = CreateGenericEvent( name );
    
    f.write("public " + type + " " + name + " = " + value + ";\n\t");
    f.write("private " + type + " revert  = " + value + ";\n\n\t");
    
    f.write("new public void OnTrigger(Huevo _huevo)\n\t");
    f.write("{\n\t\t");
    f.write("revert = _huevo." + name + ";\n\t\t");
    f.write("_huevo." + name + " = " + name + ";\n\t");
    f.write("}\n\n\t");
    
    f.write("new public void OnDetrigger(Huevo _huevo)\n\t");
    f.write("{\n\t\t");
    f.write("_huevo." + name + " = revert;\n\t");
    f.write("}\n");
    f.write("}");
    
def CreateGenericEffect( classname ):
    classname += "Effect";
    print("Creating effect " + classname + ".cs...");
    
    f = open("Assets/Scripts/Triggers/Effects/TEST/" + classname + ".cs", "w");
    
    f.write("using UnityEngine;\n\n");
    f.write("public class " + classname + " : TriggerEffect\n{\n\t");
    
    return f;
    
def CreateEffect( name, value, type ):
    f = CreateGenericEffect( name );
    
    f.write("public " + type + " " + name + " = " + value + "'\n\t");
    
    return;
    
f = open("Assets/Scripts/Huevo.cs", "r");

flag = False;
types = [];
names = [];
values = [];
for line in f:
    if not flag and not "#region Public Variables" in line:
        continue;
    if flag and "#endregion Public Variables" in line:
        break;
    
    if not flag:
        flag = True;
        continue;
    
    if "public" in line and not "()" in line:
        tmp = line.split("=")[0].replace("public ", "").strip().split(" ");
        
        types.append(tmp[0]);
        
        for s in range(1,len(tmp)):
            if(tmp[s] != ""):
                names.append(tmp[s]);
        
        values.append(line.split("=")[1].strip()[:-1]);
    
f.close();

for i in range(0, len(types)):
    CreateEvent(names[i], values[i], types[i]);
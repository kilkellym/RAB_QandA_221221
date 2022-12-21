#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RAB_QandA
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Eric's question
            // select element

            ISelectionFilter filter = new MySelectionFilter();

            Selection choice = uidoc.Selection;

            IList<Element> selection = choice.PickElementsByRectangle(filter);

            TaskDialog.Show("Test", "Selected " + selection.Count.ToString() + " windows and doors.");
            

            // Ozgur's question
            //// select element
            //Selection choice = uidoc.Selection;
            //Reference selReference = choice.PickObject(ObjectType.Element);
            ////get element from reference
            //Element e = doc.GetElement(selReference);

            //LocationCurve lc = e.Location as LocationCurve;
            //Curve curve = lc.Curve;
            //XYZ endPoint = curve.GetEndPoint(0);
            //XYZ startPoint = curve.GetEndPoint(1);

            //Family family = GetFamilyByName(doc, "W Shapes-Column");
            //FamilySymbol symbol = GetFamilySymbolByName(doc, family, "W10X33");


            //using(Transaction t = new Transaction(doc))
            //{
            //    t.Start("Add column to beam");

            //    symbol.Activate();
            //    FamilyInstance newFamInst = doc.Create.NewFamilyInstance(endPoint, symbol, level, Autodesk.Revit.DB.Structure.StructuralType.Column);

            //    t.Commit();
            //}

            // Paul's question
            //FilteredElementCollector collector = new FilteredElementCollector(doc);
            //collector.OfClass(typeof(WallType));

            //WallType curType = collector.FirstElement() as WallType;

            //using (Transaction t = new Transaction(doc))
            //{
            //    t.Start("Create new wall type");
            //    WallType newType = curType.Duplicate("New Wall Type 5") as WallType;

            //    CompoundStructure cs = newType.GetCompoundStructure();
            //    IList<CompoundStructureLayer> layerList = cs.GetLayers();
            //    //cs.SetNumberOfShellLayers(ShellLayerType.Exterior, 6);

            //    CompoundStructureLayer newLayer = new CompoundStructureLayer();
            //    newLayer.Width = .5;
            //    newLayer.Function = MaterialFunctionAssignment.Structure;

            //    CompoundStructureLayer newLayer2 = new CompoundStructureLayer();
            //    newLayer2.Width = .1;
            //    newLayer2.Function = MaterialFunctionAssignment.Finish2;

            //    layerList.Add(newLayer);
            //    layerList.Add(newLayer2);   

            //    cs.SetLayers(layerList);

            //    newType.SetCompoundStructure(cs);


            //    t.Commit();
            //}


            return Result.Succeeded;
        }

        public static FamilySymbol GetFamilySymbolByName(Document doc, Family curFam, string familySymbolName)
        {
            List<FamilySymbol> familySymbols = GetFamilySymbolsFromFamily(doc, curFam);

            foreach (FamilySymbol curFS in familySymbols)
            {
                if (curFS.Name == familySymbolName)
                    return curFS;
            }

            return null;
        }
        public static List<FamilySymbol> GetFamilySymbolsFromFamily(Document doc, Family curFam)
        {
            List<FamilySymbol> returnList = new List<FamilySymbol>();

            ISet<ElementId> fsList = curFam.GetFamilySymbolIds();

            foreach (ElementId curID in fsList)
            {
                FamilySymbol curFS = doc.GetElement(curID) as FamilySymbol;
                returnList.Add(curFS);
            }

            return returnList;
        }
        public static Family GetFamilyByName(Document doc, string familyName)
        {
            List<Family> famList = GetAllFamilies(doc);

            foreach (Family curFam in famList)
            {
                if (curFam.Name == familyName)
                    return curFam;
            }

            return null;
        }

        public static List<Family> GetAllFamilies(Document curDoc)
        {
            List<Family> returnList = new List<Family>();

            FilteredElementCollector collector = new FilteredElementCollector(curDoc);
            collector.OfClass(typeof(Family));

            foreach (Family family in collector)
            {
                returnList.Add(family);
            }

            return returnList;
        }
    }

    public class MySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category == null)
                return false;
            else if(element.Category.Name == "Doors")
                return true;
            else if (element.Category.Name == "Windows")
                return true;

            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
}

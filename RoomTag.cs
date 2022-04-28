using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace RevitAPI_AddRoomTag
{
    [Transaction(TransactionMode.Manual)]
    public class RoomTag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts1 = new Transaction(doc, "Маркировка помещений");

            ts1.Start();

            ICollection<ElementId> rooms;
            foreach (Level level in listLevel)
            {
                rooms = doc.Create.NewRooms2(level);                                    // расстановка помещений
            }
            FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms);
            IList<ElementId> roomids = FEC.ToElementIds() as IList<ElementId>;
            foreach (ElementId roomid in roomids)
            {
                Element e = doc.GetElement(roomid);                                     
                Room r = e as Room;                                                     // помещение
                string lName = r.Level.Name.Substring(6);                               // номерЭтажа
                r.Name = $"{lName}_{r.Number}";                                         // марка номерЭтажа_номерПомещения
                doc.Create.NewRoomTag(new LinkElementId(roomid), new UV(0, 0), null);   // создание марки в центре
            }

            ts1.Commit();

            return Result.Succeeded;
        }
    }
}

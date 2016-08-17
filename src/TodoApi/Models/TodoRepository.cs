using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TodoApi.Models
{
    public class TodoRepository : ITodoRepository
    {
        private const string path = @"\\WFAPPUAT01\todos\todos.xml";
        private List<TodoItem> _todos = new List<TodoItem>();

        public TodoRepository()
        {
        }

        public IEnumerable<TodoItem> GetAll()
        {
            return ReadXML();
        }

        public void Add(TodoItem item)
        {
            AddXML(item);
        }

        public TodoItem Find(int p_id)
        {
            List<TodoItem> list = ReadXML();
            TodoItem item = list.Find(i => i.Id == p_id);

            return item;
        }

        public void Remove(int p_id)
        {
            try
            {
                XDocument xd = XDocument.Load(path);

                xd.Root.Elements("Todo").Where(i => i.Element("Id").Value == p_id.ToString()).Remove();
                using (FileStream stream = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite))
                {
                    xd.Save(stream);
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public void Update(TodoItem p_item)
        {
            try
            {
                XDocument xd = XDocument.Load(path);
                var Items = from item in xd.Root.Elements("Todo")
                            where item.Element("Id").Value == p_item.Id.ToString()
                            select item;
                XElement xe = Items.FirstOrDefault();
                xe.Element("Name").SetValue(p_item.Name);
                xe.Element("isComplete").SetValue(p_item.IsComplete);
                
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    xd.Save(stream);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddXML(TodoItem p_todo)
        {
            try
            {
                XDocument xd = XDocument.Load(path);
                
                XElement xe = new XElement("Todo");
                xe.Add(new XElement("Id", p_todo.Id.ToString()));
                xe.Add(new XElement("Name", p_todo.Name));
                xe.Add(new XElement("isComplete", p_todo.IsComplete));

                xd.Root.Add(xe);
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    xd.Save(stream);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private List<TodoItem> ReadXML()
        {
            
            List<TodoItem> returnList = new List<TodoItem>();
            using (FileStream oWR = new FileStream(path, FileMode.Open))
            {
                XElement todos = XElement.Load(oWR);
                //var list = from items in todos.Elements() select items;
                foreach(XElement j in todos.Elements())
                {
                    TodoItem todoItem = new TodoItem();
                    foreach (XElement i in j.Nodes())
                    {
                        
                        switch (i.Name.LocalName)
                        {
                            case "Id":
                                todoItem.Id = Convert.ToInt32(i.Value);
                                break;
                            case "Name":
                                todoItem.Name = i.Value;
                                break;
                            case "isComplete":
                                todoItem.IsComplete = Convert.ToBoolean(i.Value);
                                break;
                        }
                    }
                    returnList.Add(todoItem);
                }
                
            }
            return returnList;
        }

        

    }
}

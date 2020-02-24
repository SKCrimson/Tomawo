using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
// ReSharper disable PossibleNullReferenceException

namespace Tomawo.Code
{
    internal class Settings
    {
        #region Constructors

        internal Settings()
        {
            Session();
        }

        #endregion

        #region Variables

        private string _profile;
        private string _project;

        private string _quiet;

        #endregion

        #region Properties

        public double Time { get; set; }
        public double SessionCont { get; set; }
        public double BreakTime { get; set; }
        public double LongBreak { get; set; }
        public bool Quiet
        {
            get => _quiet != "0";
            set => _quiet = value ? "1" : "0";
        }

        #endregion

        #region Methods

        private XDocument CurrentStateSession()
        {
            XDocument document = XDocument.Load("Xml/Settings.xml");
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XAttribute attribute in elements.Where(element => element.Name == "Current").SelectMany(element => element.Attributes()))
            {
                if (attribute.Name == "profile")
                {
                    _profile = attribute.Value;
                }
                else
                {
                    _project = attribute.Value;
                }
            }

            return document;
        }
        internal void Session()
        {
            XDocument document = CurrentStateSession();
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XAttribute attribute in from element in elements
                                             where element.Name == "Profiles"
                                             from profileElement in element.Elements()
                                             where profileElement.FirstAttribute.Value == _profile
                                             from projectElement in profileElement.Elements()
                                             where projectElement.FirstAttribute.Value == _project
                                             from attribute in projectElement.Attributes()
                                             select attribute)
            {
                if (attribute.Name == "session")
                {
                    Time = double.Parse(attribute.Value);
                }
                else if (attribute.Name == "sessionCont")
                {
                    SessionCont = double.Parse(attribute.Value);
                }
                else if (attribute.Name == "break")
                {
                    BreakTime = double.Parse(attribute.Value);
                }
                else if (attribute.Name == "longBreak")
                {
                    LongBreak = double.Parse(attribute.Value);
                }
                else if (attribute.Name == "quiet")
                {
                    _quiet = attribute.Value;
                }
            }
        }

        internal static void UpdateTime(double time)
        {
            XDocument document = XDocument.Load("Xml/Settings.xml");
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XElement element in elements.Where(element => element.Name == "Current"))
            {
                element.SetAttributeValue("profile", "user");
            }

            foreach (XElement projectElement in from element in elements
                                                where element.Name == "Profiles"
                                                from profileElement in element.Elements()
                                                where profileElement.FirstAttribute.Value == "user"
                                                from projectElement in profileElement.Elements()
                                                where projectElement.FirstAttribute.Value == "default"
                                                select projectElement)
            {
                projectElement.SetAttributeValue("session", time.ToString("##."));
            }

            document.Save("Xml/Settings.xml");
        }
        internal static void UpdateBreak(double breakTime)
        {
            XDocument document = XDocument.Load("Xml/Settings.xml");
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XElement element in elements.Where(element => element.Name == "Current"))
            {
                element.SetAttributeValue("profile", "user");
            }

            foreach (XElement projectElement in from element in elements
                                                where element.Name == "Profiles"
                                                from profileElement in element.Elements()
                                                where profileElement.FirstAttribute.Value == "user"
                                                from projectElement in profileElement.Elements()
                                                where projectElement.FirstAttribute.Value == "default"
                                                select projectElement)
            {
                projectElement.SetAttributeValue("break", breakTime.ToString("##."));
            }

            document.Save("Xml/Settings.xml");
        }
        internal static void UpdateLongBreak(double longBreak)
        {
            XDocument document = XDocument.Load("Xml/Settings.xml");
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XElement element in elements.Where(element => element.Name == "Current"))
            {
                element.SetAttributeValue("profile", "user");
            }

            foreach (XElement projectElement in from element in elements
                                                where element.Name == "Profiles"
                                                from profileElement in element.Elements()
                                                where profileElement.FirstAttribute.Value == "user"
                                                from projectElement in profileElement.Elements()
                                                where projectElement.FirstAttribute.Value == "default"
                                                select projectElement)
            {
                projectElement.SetAttributeValue("longBreak", longBreak.ToString("##."));
            }

            document.Save("Xml/Settings.xml");
        }
        internal static void UpdateSessionsCount(double count)
        {
            XDocument document = XDocument.Load("Xml/Settings.xml");
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XElement element in elements.Where(element => element.Name == "Current"))
            {
                element.SetAttributeValue("profile", "user");
            }

            foreach (XElement projectElement in from element in elements
                                                where element.Name == "Profiles"
                                                from profileElement in element.Elements()
                                                where profileElement.FirstAttribute.Value == "user"
                                                from projectElement in profileElement.Elements()
                                                where projectElement.FirstAttribute.Value == "default"
                                                select projectElement)
            {
                projectElement.SetAttributeValue("sessionCont", count.ToString("##."));
            }

            document.Save("Xml/Settings.xml");
        }
        internal static void UpdateQuiet(bool quiet)
        {
            string _quiet = quiet ? "1" : "0";

            XDocument document = XDocument.Load("Xml/Settings.xml");
            IEnumerable<XElement> elements = document.Element("Timer").Descendants();

            foreach (XElement element in elements.Where(element => element.Name == "Current"))
            {
                element.SetAttributeValue("profile", "user");
            }

            foreach (XElement projectElement in from element in elements
                                                where element.Name == "Profiles"
                                                from profileElement in element.Elements()
                                                where profileElement.FirstAttribute.Value == "user"
                                                from projectElement in profileElement.Elements()
                                                where projectElement.FirstAttribute.Value == "default"
                                                select projectElement)
            {
                projectElement.SetAttributeValue("quiet", _quiet);
            }

            document.Save("Xml/Settings.xml");
        }

        #endregion
    }
}

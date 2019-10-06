using System;
using System.Collections.Generic;
using System.Text;

namespace Design_Patterns
{
    public class Person2
    {
        public string StreetAddress, Postcode, City;
        public string CompanyName, Position;
        public int AnnualIncome;


    }

    public class CodeBuilder
    {
        private Dictionary<string, string> fieldDict = new Dictionary<string, string>();
        private string className;


        public CodeBuilder(string className)
        {
            this.className = className;
        }

        public CodeBuilder AddField(string name, string type)
        {
            fieldDict.Add(name, type);

            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");

            foreach(var pair in fieldDict)
            {
                sb.AppendLine($"  public {pair.Value} {pair.Key};");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    public class PersonBuilder2 //Facade
    {
        protected Person2 person = new Person2();

        public PersonJobBuilder2 Works => new PersonJobBuilder2(person);
        public PersonAddressBuilder2 Lives => new PersonAddressBuilder2(person);
    }

    public class PersonAddressBuilder2 : PersonBuilder2
    {
        public PersonAddressBuilder2(Person2 person)
        {
            this.person = person;
        }

        public PersonAddressBuilder2 At(string streetAddress)
        {
            person.StreetAddress = streetAddress;

            return this;
        }

        public PersonAddressBuilder2 WithPostCode(string postcode)
        {
            person.Postcode = postcode;

            return this;
        }

        public PersonAddressBuilder2 In(string city)
        {
            person.City = city;

            return this;
        }
    }

    public class PersonJobBuilder2 : PersonBuilder2
    {
        public PersonJobBuilder2(Person2 person)
        {
            this.person = person;
        }

        public PersonJobBuilder2 At(string companyName)
        {
            person.CompanyName = companyName;

            return this;
        }

        public PersonJobBuilder2 AsA(string position)
        {
            person.Position = position;

            return this;
        }

        public PersonBuilder2 Earning(int amount)
        {
            person.AnnualIncome = amount;

            return this;
        }
    }


    public class HtmlElement
    {
        public string Name = "", Text = "";
        public List<HtmlElement> Elements = new List<HtmlElement>();
        private const int indentSize = 2;

        public HtmlElement()
        {

        }

        public HtmlElement(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = new string(' ', indentSize * indent);
            sb.AppendLine($"{i}<{Name}>");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(new string(' ', indentSize * (indent + 1)));
                sb.AppendLine(Text);
            }

            foreach (var element in Elements)
            {
                sb.Append(element.ToStringImpl(indent + 1));
            }

            sb.AppendLine($"{i}</{Name}>");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    public class Person
    {
        public string Name;
        public string Position;

        public class Builder : PersonJobBuilder<Builder> { }

        public static Builder New => new Builder();

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
        }
    }

    public abstract class PersonBuilder
    {
        protected Person person = new Person();

        public Person Build()
        {
            return person;
        }
    }

    public class PersonInfoBuilder<Self> : PersonBuilder where Self : PersonInfoBuilder<Self>
    {
        

        public Self Called(string name)
        {
            person.Name = name;

            return (Self) this;
        }
    }

    public class PersonJobBuilder<Self> : PersonInfoBuilder<PersonJobBuilder<Self>> where Self : PersonJobBuilder<Self>
    {
        public Self WorksAsA(string position)
        {
            person.Position = position;

            return (Self) this;
        }
    }

    public class HtmlBuilder
    {
        private readonly string rootName;
        private HtmlElement root = new HtmlElement();


        public HtmlBuilder(string rootName)
        {
            this.rootName = rootName;
            root.Name = rootName;
        }

        public HtmlBuilder AddChild(string childName, string childText)
        {
            var e = new HtmlElement(childName, childText);
            root.Elements.Add(e);

            return this;
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public void Clear()
        {
            root = new HtmlElement { Name = rootName };
        }
    }
}

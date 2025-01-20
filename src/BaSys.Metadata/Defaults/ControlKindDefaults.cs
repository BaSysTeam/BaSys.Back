using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Defaults
{
    public static class ControlKindDefaults
    {
        public static readonly ControlKind PrimeVueTextInput = new ControlKind()
        {
            Uid = new Guid("{0923119C-04F8-4CE4-A417-777D93011312}"),
            Name = "PrimeVue.TextInput",
            Title = "Text input",
            AppliesFor = new List<DataType> { DataTypeDefaults.String }
        };

        public static readonly ControlKind PrimeVueTextArea = new ControlKind()
        {
            Uid = new Guid("{980985DD-60AC-43B7-A5DA-2AF54D6FD1CE}"),
            Name = "PrimeVue.TextArea",
            Title = "Text area",
            AppliesFor = new List<DataType> { DataTypeDefaults.String }
        };

        public static readonly ControlKind PrimeVueIntegerInput = new ControlKind()
        {
            Uid = new Guid("{95BE8B41-74E5-4DB9-9A6D-648437F39F70}"),
            Name = "PrimeVue.IntegerInput",
            Title = "Integer input",
            AppliesFor = new List<DataType> { DataTypeDefaults.Int, DataTypeDefaults.Long }
        };

        public static readonly ControlKind PrimeVueNumberInput = new ControlKind()
        {
            Uid = new Guid("{E0776FA7-8462-494D-8275-D9F9D19DC9EC}"),
            Name = "PrimeVue.NumberInput",
            Title = "Number input",
            AppliesFor = new List<DataType> { DataTypeDefaults.Decimal }
        };

        public static readonly ControlKind PrimeVueCheckbox = new ControlKind()
        {
            Uid = new Guid("{50989636-3940-461A-89DB-5DE709B58F31}"),
            Name = "PrimeVue.Checkbox",
            Title = "Checkbox",
            AppliesFor = new List<DataType> { DataTypeDefaults.Bool }
        };

        public static readonly ControlKind PrimeVueSwitchInput = new ControlKind()
        {
            Uid = new Guid("{80EA55AF-19FE-48E6-8F53-5091DA45C6B7}"),
            Name = "PrimeVue.SwitchInput",
            Title = "Switch",
            AppliesFor = new List<DataType> { DataTypeDefaults.Bool }
        };

        public static readonly ControlKind PrimeVueDateInput = new ControlKind()
        {
            Uid = new Guid("{909ADF77-0B49-4A32-8A24-9E29429A2D90}"),
            Name = "PrimeVue.DateInput",
            Title = "Date input",
            AppliesFor = new List<DataType> { DataTypeDefaults.DateTime }
        };

        public static readonly ControlKind PrimeVueDateTimeInput = new ControlKind()
        {
            Uid = new Guid("{EE2CAC4F-0749-4131-93F8-71BD5BE399E6}"),
            Name = "PrimeVue.DateTimeInput",
            Title = "Date&time input",
            AppliesFor = new List<DataType> { DataTypeDefaults.DateTime }
        };
    }
}

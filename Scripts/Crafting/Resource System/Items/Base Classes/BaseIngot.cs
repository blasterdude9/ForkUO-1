﻿using System;
using System.Linq;

using Server.Resources;

namespace Server.Items
{
    public abstract class BaseIngot : Item, IResource, ICommodity
    {
        private ResourceInfo _info;
        public ResourceInfo Info
        {
            get { return _info; }
            set
            {
                _info = value;
                Hue = _info.Hue;

                InvalidateProperties();
            }
        }

        private object _PreloadResource = null;
        public object PreloadResource { get { return _PreloadResource; } }

        public static String ResourceGroupName { get { return "Metal"; } }
        public String ResourceGroup { get { return ResourceGroupName; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ResourceString Resource
        {
            get { return new ResourceString(ResourceGroup, _info.ResourceName); }
            set { SetResource = value.Value; }
        }

        private String SetResource { set { Info = ResourceService.Service.LookupResource(ResourceGroup, value); } }

        public BaseIngot(String resource) : this(resource, 1) { }

        public BaseIngot(String resource, Int32 amount) : base(0x1BF2)
        {
            SetResource = resource;
            Stackable = true;
            Amount = amount;
            Hue = _info.Hue;
        }

        public BaseIngot(Serial serial) : base(serial)
        {
        }

        public override Double DefaultWeight { get { return 0.1; } }
        public override Int32 LabelNumber { get { return _info.ProcessedNumber; } }
        Int32 ICommodity.DescriptionNumber { get { return this.LabelNumber; } }
        Boolean ICommodity.IsDeedable { get { return true; } }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.Amount > 1)
                list.Add(1050039, "{0}\t#{1}", this.Amount, 1027154); // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add(1027154); // ingots
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_info.Hue != 0x0)
            {
                if (_info.ResourceNumber > 0)
                    list.Add(_info.ResourceNumber);
                else
                    list.Add(_info.ResourceName);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((Int32)2); // version

            writer.Write((String)_info.ResourceName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Int32 version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    _PreloadResource = reader.ReadString();

                    break;
                case 1:
                    _PreloadResource = new object[] { "OR", reader.ReadInt() };

                    break;
                case 0:
                    _PreloadResource = new object[] { "OI", reader.ReadInt() };

                    break;
            }
        }
    }
}
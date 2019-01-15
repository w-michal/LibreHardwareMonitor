/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2010 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenHardwareMonitor.Hardware {
    public struct ParameterDescription {
    private readonly string name;
    private readonly string description;
    private readonly float defaultValue;    

    public ParameterDescription(string name, string description, 
      float defaultValue) {
      this.name = name;
      this.description = description;
      this.defaultValue = defaultValue;
    }

    public string Name { get { return name; } }

    public string Description { get { return description; } }

    public float DefaultValue { get { return defaultValue; } }
  }

  internal class Parameter : IParameter {
    private readonly ISensor sensor;
    private ParameterDescription description;
    private float value;
    private bool isDefault;
    private readonly IDictionary<string, string> settings;

    public Parameter(ParameterDescription description, ISensor sensor,
      IDictionary<string, string> settings) 
    {
      this.sensor = sensor;
      this.description = description;
      this.settings = settings;
      this.isDefault = !settings.ContainsKey(Identifier.ToString());
      this.value = description.DefaultValue;
      if (!this.isDefault) {
        settings.TryGetValue(Identifier.ToString(), out string svalue);
        if (!float.TryParse(svalue,
          NumberStyles.Float,
          CultureInfo.InvariantCulture,
          out this.value))
          this.value = description.DefaultValue;
      }
    }

    public ISensor Sensor {
      get {
        return sensor;
      }
    }

    public Identifier Identifier {
      get {
        return new Identifier(sensor.Identifier, "parameter",
          Name.Replace(" ", "").ToLowerInvariant());
      }
    }

    public string Name { get { return description.Name; } }

    public string Description { get { return description.Description; } }

    public float Value {
      get {
        return value;
      }
      set {
        this.isDefault = false;
        this.value = value;
        this.settings[Identifier.ToString()] = value.ToString(
          CultureInfo.InvariantCulture);
      }
    }

    public float DefaultValue { 
      get { return description.DefaultValue; } 
    }

    public bool IsDefault {
      get { return isDefault; }
      set {
        this.isDefault = value;
        if (value) {
          this.value = description.DefaultValue;
          this.settings.Remove(Identifier.ToString());
        }
      }
    }

    public void Accept(IVisitor visitor) {
      if (visitor == null)
        throw new ArgumentNullException("visitor");
      visitor.VisitParameter(this);
    }

    public void Traverse(IVisitor visitor) { }
  }
}

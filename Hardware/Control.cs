﻿/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2010-2014 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenHardwareMonitor.Hardware {

  internal delegate void ControlEventHandler(Control control);

  internal class Control : IControl {

    private readonly Identifier identifier;
    private readonly IDictionary<string, string> settings;
    private ControlMode mode;
    private float softwareValue;
    private float minSoftwareValue;
    private float maxSoftwareValue;

    public Control(ISensor sensor, IDictionary<string, string> settings, float minSoftwareValue,
      float maxSoftwareValue) 
    {
      this.identifier = new Identifier(sensor.Identifier, "control");
      this.settings = settings;
      this.minSoftwareValue = minSoftwareValue;
      this.maxSoftwareValue = maxSoftwareValue;
      settings.TryGetValue(new Identifier(identifier, "value").ToString(), out string valuestr);
      float.TryParse(valuestr, NumberStyles.Float, CultureInfo.InvariantCulture, out this.softwareValue);

      int mode;
      settings.TryGetValue(new Identifier(identifier, "mode").ToString(), out valuestr);
      if (!int.TryParse(valuestr, NumberStyles.Integer, CultureInfo.InvariantCulture, out mode)) 
      {
        this.mode = ControlMode.Undefined;
      } else {
        this.mode = (ControlMode)mode;
      }
    }

    public Identifier Identifier {
      get {
        return identifier;
      }
    }

    public ControlMode ControlMode {
      get {
        return mode;
      }
      private set {
        if (mode != value) {
          mode = value;
          if (ControlModeChanged != null)
            ControlModeChanged(this);
          this.settings[new Identifier(identifier, "mode").ToString()] =
            ((int)mode).ToString(CultureInfo.InvariantCulture);
        }
      }
    }

    public float SoftwareValue {
      get {
        return softwareValue;
      }
      private set {
        if (softwareValue != value) {
          softwareValue = value;
          if (SoftwareControlValueChanged != null)
            SoftwareControlValueChanged(this);
          this.settings[new Identifier(identifier,
            "value").ToString()] =
            value.ToString(CultureInfo.InvariantCulture);
        }
      }
    }

    public void SetDefault() {
      ControlMode = ControlMode.Default;
    }

    public float MinSoftwareValue {
      get {
        return minSoftwareValue;
      }
    }

    public float MaxSoftwareValue {
      get {
        return maxSoftwareValue;
      }
    }

    public void SetSoftware(float value) {
      ControlMode = ControlMode.Software;
      SoftwareValue = value;
    }

    internal event ControlEventHandler ControlModeChanged;
    internal event ControlEventHandler SoftwareControlValueChanged;
  }
}

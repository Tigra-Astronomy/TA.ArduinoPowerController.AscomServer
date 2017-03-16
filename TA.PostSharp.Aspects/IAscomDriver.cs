// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: IAscomDriver.cs  Last modified: 2017-03-16@23:34 by Tim Long

namespace TA.PostSharp.Aspects
    {
    public interface IAscomDriver
        {
        bool Connected { get; }
        }
    }
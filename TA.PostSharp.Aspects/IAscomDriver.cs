// This file is part of the GTD.Integra.FocusingRotator project
// 
// Copyright © 2016-2017 Tigra Astronomy., all rights reserved.
// 
// File: IAscomDriver.cs  Last modified: 2017-02-13@01:06 by Tim Long

namespace TA.PostSharp.Aspects
    {
    public interface IAscomDriver
        {
        bool Connected { get; }
        }
    }
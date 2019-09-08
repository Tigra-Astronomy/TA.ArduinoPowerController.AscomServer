// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: GitVersionExtensions.cs  Last modified: 2019-09-08@04:46 by Tim Long

using System;
using System.Linq;
using System.Reflection;

namespace TA.ArduinoPowerController.Server
{
    public static class GitVersionExtensions
    {
        public static string GitBuildMetadata => GitVersion().GitVersionField("FullBuildMetaData");

        public static string GitCommitDate => GitVersion().GitVersionField("CommitDate");

        public static string GitCommitSha => GitVersion().GitVersionField("Sha");

        public static string GitCommitShortSha => GitVersion().GitVersionField("ShortSha");

        public static string GitFullSemVer => GitVersion().GitVersionField("FullSemVer");

        public static string GitInformationalVersion => GitVersion().GitVersionField("InformationalVersion");

        public static string GitMajorVersion => GitVersion().GitVersionField("Major");

        public static string GitMinorVersion => GitVersion().GitVersionField("Minor");

        public static string GitPatchVersion => GitVersion().GitVersionField("Patch");

        public static string GitSemVer => GitVersion().GitVersionField("SemVer");

        private static Type GitVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == "GitVersionInformation");
            return type;
        }

        private static string GitVersionField(this Type gitVersionInformationType, string fieldName)
        {
            var versionField = gitVersionInformationType?.GetField(fieldName);
            return versionField?.GetValue(null).ToString() ?? "undefined";
        }
    }
}
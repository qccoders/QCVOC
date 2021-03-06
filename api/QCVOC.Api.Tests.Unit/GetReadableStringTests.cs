﻿// <copyright file="GetReadableStringTests.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace Server.Tests
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common;
    using Xunit;

    public class GetReadableStringTests
    {
        [Fact]
        public void NullDictionary()
        {
            ModelStateDictionary modelState = null;
            string result = modelState.GetReadableString();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ValidDictionary()
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            string result = modelState.GetReadableString();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SingleError()
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "The name is too short");
            string result = modelState.GetReadableString();

            Assert.Equal("Name: The name is too short", result);
        }

        [Fact]
        public void OneKeyTwoErrors()
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Not capitalized.");
            modelState.AddModelError("Name", "Too short.");
            string result = modelState.GetReadableString();

            Assert.Equal("Name: Not capitalized, Too short", result);
        }

        [Fact]
        public void TwoKeysTwoErrors()
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Too short.");
            modelState.AddModelError("Password", "Too long.");
            string result = modelState.GetReadableString();

            Assert.Equal("Name: Too short; Password: Too long", result);
        }

        [Fact]
        public void ThreeKeysMultiErrors()
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Too short.");
            modelState.AddModelError("Password", "Too long");
            modelState.AddModelError("Password", "Not complex enough.");
            modelState.AddModelError("Password", "Already used");
            modelState.AddModelError("PrimaryPhone", "Invalid.");
            string result = modelState.GetReadableString();

            Assert.Equal(
                "Name: Too short; "
                + "Password: Too long, Not complex enough, Already used; "
                + "PrimaryPhone: Invalid", result);
        }
    }
}

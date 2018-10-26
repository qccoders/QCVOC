// <copyright file="VerificationMethod.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans
{
    /// <summary>
    ///     Describes the method used to verify Veteran eligibiliy.
    /// </summary>
    public enum VerificationMethod
    {
        /// <summary>
        ///     The Veteran has not been verified.
        /// </summary>
        Unverified = 0,

        /// <summary>
        ///     A state issued photo Id and a military DD214 form.
        /// </summary>
        StateIdAndDD214 = 1,

        /// <summary>
        ///     An Iowa driver's license with Veteran insignia.
        /// </summary>
        IowaVeteranDL = 2,

        /// <summary>
        ///     An active military Id.
        /// </summary>
        MilitaryId = 3,
    }
}

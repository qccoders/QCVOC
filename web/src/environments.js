/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

export const PROD = {
    name: 'prod',
    hostname: 'qcvoc-prod.s3-website.us-east-1.amazonaws.com/',
    apiRoot: 'https://lv8t84qik7.execute-api.us-east-2.amazonaws.com/prod/api',
    color: 'primary',
};

export const DEV = {
    name: 'dev',
    hostname: 'qcvoc-dev.s3-website-us-east-1.amazonaws.com',
    apiRoot: 'https://qz8rgrk8j9.execute-api.us-east-2.amazonaws.com/dev/api',
    color: 'default',
};

// to debug locally, swap this configuration for DEV.
export const LOCAL = {
    name: 'local',
    hostname: 'localhost',
    apiRoot: 'https://localhost:5000/api',
    color: 'secondary',
};

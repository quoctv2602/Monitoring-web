import * as appSettings from '../../../appsettings.json';
const cloneAppSettings = appSettings;
const azureAD = cloneAppSettings.IDP;

export const globalsettings = {
  apiUrl: 'https://localhost:7090/api/',
  configUrl: '/',
  IDP: {
    TenantId: azureAD.TenantId,
    Authority: azureAD.Authority,
    ClientId: azureAD.ClientId,
    CallbackPath: azureAD.CallbackPath,
    SignedOutCallbackPath: azureAD.SignedOutCallbackPath,
  },
};

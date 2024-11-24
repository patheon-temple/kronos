/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface AuthenticationPostRequest {
  credentialsType?: CredentialsType;
  requestedScopes?: string[] | null;
  password?: string | null;
  deviceId?: string | null;
  username?: string | null;
  /** @format uuid */
  serviceId?: string | null;
  authorizationCode?: string | null;
  /** @format uuid */
  audience?: string;
}

export interface AuthenticationSuccessfulResponse {
  accessToken?: string | null;
  userId?: string | null;
  scopes?: string[] | null;
  username?: string | null;
}

export interface CreateServiceAccountRequest {
  /** @maxLength 256 */
  serviceName?: string | null;
  scopes?: string[] | null;
}

export interface CreateUserAccountRequest {
  username?: string | null;
  password?: string | null;
  deviceId?: string | null;
  scopes?: string[] | null;
}

/** @format int32 */
export enum CredentialsType {
  Unknown = "Unknown",
  DeviceId = "DeviceId",
  Password = "Password",
  AuthorizationCode = "AuthorizationCode",
}

export interface ServiceDescription {
  url?: string | null;
  description?: string | null;
  healthcheckUrl?: string | null;
}

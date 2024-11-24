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

import { AuthenticationPostRequest, AuthenticationSuccessfulResponse } from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Hermes<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * @description Authenticate user
   *
   * @tags Hermes
   * @name Authenticate
   * @request POST:/hermes/api/v1/authenticate
   * @response `200` `AuthenticationSuccessfulResponse` OK
   */
  authenticate = (data: AuthenticationPostRequest, params: RequestParams = {}) =>
    this.request<AuthenticationSuccessfulResponse, any>({
      path: `/hermes/api/v1/authenticate`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Hermes
   * @name ApiV1IntrospectionList
   * @request GET:/hermes/api/v1/introspection
   * @response `200` `void` Success
   */
  apiV1IntrospectionList = (params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/hermes/api/v1/introspection`,
      method: "GET",
      ...params,
    });
  /**
   * No description
   *
   * @tags Hermes
   * @name ApiAdminV1AudienceDetail
   * @request GET:/hermes/api/admin/v1/audience/{id}
   * @response `200` `void` Success
   */
  apiAdminV1AudienceDetail = (id: string, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/hermes/api/admin/v1/audience/${id}`,
      method: "GET",
      ...params,
    });
}

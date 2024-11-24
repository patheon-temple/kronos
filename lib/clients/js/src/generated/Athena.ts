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

import { CreateServiceAccountRequest, CreateUserAccountRequest } from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Athena<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Athena
   * @name ApiV1AccountUserPasswordPartialUpdate
   * @request PATCH:/athena/api/v1/account/user/password
   * @response `200` `void` Success
   */
  apiV1AccountUserPasswordPartialUpdate = (params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/athena/api/v1/account/user/password`,
      method: "PATCH",
      ...params,
    });
  /**
   * No description
   *
   * @tags Athena
   * @name ApiAdminV1AccountUserCreate
   * @request POST:/athena/api/admin/v1/account/user
   * @response `200` `void` Success
   */
  apiAdminV1AccountUserCreate = (data: CreateUserAccountRequest, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/athena/api/admin/v1/account/user`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Athena
   * @name ApiAdminV1AccountServiceCreate
   * @request POST:/athena/api/admin/v1/account/service
   * @response `200` `void` Success
   */
  apiAdminV1AccountServiceCreate = (data: CreateServiceAccountRequest, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/athena/api/admin/v1/account/service`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Athena
   * @name GetUserAccountByIdAsync
   * @request GET:/athena/api/admin/v1/account/user/{id}
   * @response `200` `void` Success
   */
  getUserAccountByIdAsync = (id: string, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/athena/api/admin/v1/account/user/${id}`,
      method: "GET",
      ...params,
    });
  /**
   * No description
   *
   * @tags Athena
   * @name GetServiceAccountByIdAsync
   * @request GET:/athena/api/admin/v1/account/service/{id}
   * @response `200` `void` Success
   */
  getServiceAccountByIdAsync = (id: string, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/athena/api/admin/v1/account/service/${id}`,
      method: "GET",
      ...params,
    });
}

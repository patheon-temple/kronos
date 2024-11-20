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

import { ServiceDescription } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Kronos<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * @description Services discovery endpoint
   *
   * @tags Kronos
   * @name Discovery
   * @request GET:/kronos/api/v1
   * @response `200` `Record<string,ServiceDescription>` OK
   */
  discovery = (params: RequestParams = {}) =>
    this.request<Record<string, ServiceDescription>, any>({
      path: `/kronos/api/v1`,
      method: "GET",
      format: "json",
      ...params,
    });
}

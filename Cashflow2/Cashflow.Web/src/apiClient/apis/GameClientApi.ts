/* tslint:disable */
/* eslint-disable */
/**
 * Cashflow API v1
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';
import type {
  GameModel,
} from '../models/index';
import {
    GameModelFromJSON,
    GameModelToJSON,
} from '../models/index';

export interface HubsGameClientErrorPostRequest {
    message?: string;
}

export interface HubsGameClientGameStateUpdatedPostRequest {
    game?: GameModel;
}

/**
 * 
 */
export class GameClientApi extends runtime.BaseAPI {

    /**
     */
    async hubsGameClientErrorPostRaw(requestParameters: HubsGameClientErrorPostRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        if (requestParameters['message'] != null) {
            queryParameters['message'] = requestParameters['message'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/hubs/GameClient/Error`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async hubsGameClientErrorPost(requestParameters: HubsGameClientErrorPostRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<void> {
        await this.hubsGameClientErrorPostRaw(requestParameters, initOverrides);
    }

    /**
     */
    async hubsGameClientGameStateUpdatedPostRaw(requestParameters: HubsGameClientGameStateUpdatedPostRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        if (requestParameters['game'] != null) {
            queryParameters['game'] = requestParameters['game'];
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/hubs/GameClient/GameStateUpdated`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async hubsGameClientGameStateUpdatedPost(requestParameters: HubsGameClientGameStateUpdatedPostRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<void> {
        await this.hubsGameClientGameStateUpdatedPostRaw(requestParameters, initOverrides);
    }

}

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

import { mapValues } from '../runtime';
import type { PlayerModel } from './PlayerModel';
import {
    PlayerModelFromJSON,
    PlayerModelFromJSONTyped,
    PlayerModelToJSON,
    PlayerModelToJSONTyped,
} from './PlayerModel';
import type { PlayerOptionsModel } from './PlayerOptionsModel';
import {
    PlayerOptionsModelFromJSON,
    PlayerOptionsModelFromJSONTyped,
    PlayerOptionsModelToJSON,
    PlayerOptionsModelToJSONTyped,
} from './PlayerOptionsModel';
import type { GameModel } from './GameModel';
import {
    GameModelFromJSON,
    GameModelFromJSONTyped,
    GameModelToJSON,
    GameModelToJSONTyped,
} from './GameModel';

/**
 * 
 * @export
 * @interface GameResponseModel
 */
export interface GameResponseModel {
    /**
     * 
     * @type {boolean}
     * @memberof GameResponseModel
     */
    isSuccess?: boolean;
    /**
     * 
     * @type {string}
     * @memberof GameResponseModel
     */
    message?: string | null;
    /**
     * 
     * @type {PlayerModel}
     * @memberof GameResponseModel
     */
    player?: PlayerModel;
    /**
     * 
     * @type {PlayerOptionsModel}
     * @memberof GameResponseModel
     */
    playerOptions?: PlayerOptionsModel;
    /**
     * 
     * @type {GameModel}
     * @memberof GameResponseModel
     */
    game?: GameModel;
}

/**
 * Check if a given object implements the GameResponseModel interface.
 */
export function instanceOfGameResponseModel(value: object): value is GameResponseModel {
    return true;
}

export function GameResponseModelFromJSON(json: any): GameResponseModel {
    return GameResponseModelFromJSONTyped(json, false);
}

export function GameResponseModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): GameResponseModel {
    if (json == null) {
        return json;
    }
    return {
        
        'isSuccess': json['isSuccess'] == null ? undefined : json['isSuccess'],
        'message': json['message'] == null ? undefined : json['message'],
        'player': json['player'] == null ? undefined : PlayerModelFromJSON(json['player']),
        'playerOptions': json['playerOptions'] == null ? undefined : PlayerOptionsModelFromJSON(json['playerOptions']),
        'game': json['game'] == null ? undefined : GameModelFromJSON(json['game']),
    };
}

export function GameResponseModelToJSON(json: any): GameResponseModel {
    return GameResponseModelToJSONTyped(json, false);
}

export function GameResponseModelToJSONTyped(value?: GameResponseModel | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'isSuccess': value['isSuccess'],
        'message': value['message'],
        'player': PlayerModelToJSON(value['player']),
        'playerOptions': PlayerOptionsModelToJSON(value['playerOptions']),
        'game': GameModelToJSON(value['game']),
    };
}


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
import type { LiabilityModel } from './LiabilityModel';
import {
    LiabilityModelFromJSON,
    LiabilityModelFromJSONTyped,
    LiabilityModelToJSON,
    LiabilityModelToJSONTyped,
} from './LiabilityModel';
import type { ProfessionModel } from './ProfessionModel';
import {
    ProfessionModelFromJSON,
    ProfessionModelFromJSONTyped,
    ProfessionModelToJSON,
    ProfessionModelToJSONTyped,
} from './ProfessionModel';
import type { AssetModel } from './AssetModel';
import {
    AssetModelFromJSON,
    AssetModelFromJSONTyped,
    AssetModelToJSON,
    AssetModelToJSONTyped,
} from './AssetModel';

/**
 * 
 * @export
 * @interface PlayerModel
 */
export interface PlayerModel {
    /**
     * 
     * @type {string}
     * @memberof PlayerModel
     */
    id?: string;
    /**
     * 
     * @type {string}
     * @memberof PlayerModel
     */
    name?: string | null;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    boardSpaceId?: number;
    /**
     * 
     * @type {ProfessionModel}
     * @memberof PlayerModel
     */
    profession?: ProfessionModel;
    /**
     * 
     * @type {Array<AssetModel>}
     * @memberof PlayerModel
     */
    assets?: Array<AssetModel> | null;
    /**
     * 
     * @type {Array<LiabilityModel>}
     * @memberof PlayerModel
     */
    liabilities?: Array<LiabilityModel> | null;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    numberOfChildren?: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    cash?: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    readonly income?: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    readonly taxes?: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    readonly childExpenses?: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    readonly expenses?: number;
    /**
     * 
     * @type {number}
     * @memberof PlayerModel
     */
    readonly netIncome?: number;
}

/**
 * Check if a given object implements the PlayerModel interface.
 */
export function instanceOfPlayerModel(value: object): value is PlayerModel {
    return true;
}

export function PlayerModelFromJSON(json: any): PlayerModel {
    return PlayerModelFromJSONTyped(json, false);
}

export function PlayerModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): PlayerModel {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'] == null ? undefined : json['id'],
        'name': json['name'] == null ? undefined : json['name'],
        'boardSpaceId': json['boardSpaceId'] == null ? undefined : json['boardSpaceId'],
        'profession': json['profession'] == null ? undefined : ProfessionModelFromJSON(json['profession']),
        'assets': json['assets'] == null ? undefined : ((json['assets'] as Array<any>).map(AssetModelFromJSON)),
        'liabilities': json['liabilities'] == null ? undefined : ((json['liabilities'] as Array<any>).map(LiabilityModelFromJSON)),
        'numberOfChildren': json['numberOfChildren'] == null ? undefined : json['numberOfChildren'],
        'cash': json['cash'] == null ? undefined : json['cash'],
        'income': json['income'] == null ? undefined : json['income'],
        'taxes': json['taxes'] == null ? undefined : json['taxes'],
        'childExpenses': json['childExpenses'] == null ? undefined : json['childExpenses'],
        'expenses': json['expenses'] == null ? undefined : json['expenses'],
        'netIncome': json['netIncome'] == null ? undefined : json['netIncome'],
    };
}

export function PlayerModelToJSON(json: any): PlayerModel {
    return PlayerModelToJSONTyped(json, false);
}

export function PlayerModelToJSONTyped(value?: Omit<PlayerModel, 'income'|'taxes'|'childExpenses'|'expenses'|'netIncome'> | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'id': value['id'],
        'name': value['name'],
        'boardSpaceId': value['boardSpaceId'],
        'profession': ProfessionModelToJSON(value['profession']),
        'assets': value['assets'] == null ? undefined : ((value['assets'] as Array<any>).map(AssetModelToJSON)),
        'liabilities': value['liabilities'] == null ? undefined : ((value['liabilities'] as Array<any>).map(LiabilityModelToJSON)),
        'numberOfChildren': value['numberOfChildren'],
        'cash': value['cash'],
    };
}


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
 * @interface ProfessionModel
 */
export interface ProfessionModel {
    /**
     * 
     * @type {string}
     * @memberof ProfessionModel
     */
    name?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ProfessionModel
     */
    salary?: number;
    /**
     * 
     * @type {number}
     * @memberof ProfessionModel
     */
    childExpense?: number;
    /**
     * 
     * @type {number}
     * @memberof ProfessionModel
     */
    otherExpenses?: number;
    /**
     * 
     * @type {number}
     * @memberof ProfessionModel
     */
    savings?: number;
    /**
     * 
     * @type {Array<AssetModel>}
     * @memberof ProfessionModel
     */
    assets?: Array<AssetModel> | null;
    /**
     * 
     * @type {Array<LiabilityModel>}
     * @memberof ProfessionModel
     */
    liabilities?: Array<LiabilityModel> | null;
}

/**
 * Check if a given object implements the ProfessionModel interface.
 */
export function instanceOfProfessionModel(value: object): value is ProfessionModel {
    return true;
}

export function ProfessionModelFromJSON(json: any): ProfessionModel {
    return ProfessionModelFromJSONTyped(json, false);
}

export function ProfessionModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): ProfessionModel {
    if (json == null) {
        return json;
    }
    return {
        
        'name': json['name'] == null ? undefined : json['name'],
        'salary': json['salary'] == null ? undefined : json['salary'],
        'childExpense': json['childExpense'] == null ? undefined : json['childExpense'],
        'otherExpenses': json['otherExpenses'] == null ? undefined : json['otherExpenses'],
        'savings': json['savings'] == null ? undefined : json['savings'],
        'assets': json['assets'] == null ? undefined : ((json['assets'] as Array<any>).map(AssetModelFromJSON)),
        'liabilities': json['liabilities'] == null ? undefined : ((json['liabilities'] as Array<any>).map(LiabilityModelFromJSON)),
    };
}

export function ProfessionModelToJSON(json: any): ProfessionModel {
    return ProfessionModelToJSONTyped(json, false);
}

export function ProfessionModelToJSONTyped(value?: ProfessionModel | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'name': value['name'],
        'salary': value['salary'],
        'childExpense': value['childExpense'],
        'otherExpenses': value['otherExpenses'],
        'savings': value['savings'],
        'assets': value['assets'] == null ? undefined : ((value['assets'] as Array<any>).map(AssetModelToJSON)),
        'liabilities': value['liabilities'] == null ? undefined : ((value['liabilities'] as Array<any>).map(LiabilityModelToJSON)),
    };
}


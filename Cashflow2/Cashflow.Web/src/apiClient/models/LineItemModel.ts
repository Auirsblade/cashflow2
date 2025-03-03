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
import type { LineItemTypeModel } from './LineItemTypeModel';
import {
    LineItemTypeModelFromJSON,
    LineItemTypeModelFromJSONTyped,
    LineItemTypeModelToJSON,
    LineItemTypeModelToJSONTyped,
} from './LineItemTypeModel';

/**
 * 
 * @export
 * @interface LineItemModel
 */
export interface LineItemModel {
    /**
     * 
     * @type {string}
     * @memberof LineItemModel
     */
    name?: string | null;
    /**
     * 
     * @type {number}
     * @memberof LineItemModel
     */
    amount?: number;
    /**
     * 
     * @type {LineItemTypeModel}
     * @memberof LineItemModel
     */
    type?: LineItemTypeModel;
}



/**
 * Check if a given object implements the LineItemModel interface.
 */
export function instanceOfLineItemModel(value: object): value is LineItemModel {
    return true;
}

export function LineItemModelFromJSON(json: any): LineItemModel {
    return LineItemModelFromJSONTyped(json, false);
}

export function LineItemModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): LineItemModel {
    if (json == null) {
        return json;
    }
    return {
        
        'name': json['name'] == null ? undefined : json['name'],
        'amount': json['amount'] == null ? undefined : json['amount'],
        'type': json['type'] == null ? undefined : LineItemTypeModelFromJSON(json['type']),
    };
}

export function LineItemModelToJSON(json: any): LineItemModel {
    return LineItemModelToJSONTyped(json, false);
}

export function LineItemModelToJSONTyped(value?: LineItemModel | null, ignoreDiscriminator: boolean = false): any {
    if (value == null) {
        return value;
    }

    return {
        
        'name': value['name'],
        'amount': value['amount'],
        'type': LineItemTypeModelToJSON(value['type']),
    };
}


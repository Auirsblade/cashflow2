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


/**
 * 
 * @export
 */
export const LineItemTypeModel = {
    NUMBER_0: 0,
    NUMBER_1: 1
} as const;
export type LineItemTypeModel = typeof LineItemTypeModel[keyof typeof LineItemTypeModel];


export function instanceOfLineItemTypeModel(value: any): boolean {
    for (const key in LineItemTypeModel) {
        if (Object.prototype.hasOwnProperty.call(LineItemTypeModel, key)) {
            if (LineItemTypeModel[key as keyof typeof LineItemTypeModel] === value) {
                return true;
            }
        }
    }
    return false;
}

export function LineItemTypeModelFromJSON(json: any): LineItemTypeModel {
    return LineItemTypeModelFromJSONTyped(json, false);
}

export function LineItemTypeModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): LineItemTypeModel {
    return json as LineItemTypeModel;
}

export function LineItemTypeModelToJSON(value?: LineItemTypeModel | null): any {
    return value as any;
}

export function LineItemTypeModelToJSONTyped(value: any, ignoreDiscriminator: boolean): LineItemTypeModel {
    return value as LineItemTypeModel;
}


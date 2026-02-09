declare module '@/lib/signalR' {
    import {Ref} from 'vue';

    export interface SignalRConnection {
        start: () => Promise<void>;
        connection: any; // Replace `any` with the actual connection type if known
        status: Ref<string>;
    }

    export interface SignalRInvoke {
        execute: (...args: any[]) => Promise<any>; // Replace `any` with more specific types
        data: Ref<any>; // Replace `any` with the expected returned data type
    }

    export interface SignalROn {
        execute: (...args: any[]) => void;
    }

    export const HubConnectionState: {
        Disconnected: string;
        Connecting: string;
        Connected: string;
        Disconnecting: string;
        Reconnecting: string;
    };

    export function useSignalR(url: string): SignalRConnection;

    export function useSignalRInvoke(
        connection: SignalRConnection['connection'],
        methodName: string
    ): SignalRInvoke;

    export function useSignalROn(
        connection: SignalRConnection['connection'],
        eventName: string,
        callback: (...args: any[]) => void
    ): void;
}
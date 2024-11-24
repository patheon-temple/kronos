import {Kronos} from "./generated/Kronos";
import {ServiceDescription} from "./generated/data-contracts";
import {Athena} from "./generated/Athena";
import {Hermes} from "./generated/Hermes";

export declare type ClientName = 'Athena' | 'Hermes';

export async function createClientAsync<T = Athena | Hermes>(discoveryUrl: string, clientName: ClientName, params: any): Promise<T> {
    const kronos = new Kronos({
        baseURL: discoveryUrl
    });
    const {data} = await kronos.discovery()

    const {url} = data[clientName] as ServiceDescription;

    switch (clientName) {
        case "Athena":
            return new Athena({
                baseURL: url!,
                ...params,
            }) as T
        case "Hermes":
            return new Hermes({
                baseURL: url!,
                ...params,
            }) as T
        default:
            throw new Error(`Client name: ${clientName} not supported`);
    }
}

export type empty = Record<string, never>;

export type Dictionary<T> = {
    [index: string]: T;
};
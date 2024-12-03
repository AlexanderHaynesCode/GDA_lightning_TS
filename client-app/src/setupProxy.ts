import { createProxyMiddleware, Options } from 'http-proxy-middleware';
import { env } from 'process';
import { Application, Request, Response } from 'express';

const target: string = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:22650';

const context: string[] = [
    "/user"
];

const onError = (err: Error, req: Request, resp: Response, target: string): void => {
    console.error(`${err.message}`);
}

export default function setupProxy(app: Application): void {
    const appProxy = createProxyMiddleware(context, {
        proxyTimeout: 10000,
        target: target,
        onError: onError,
        secure: false,
        changeOrigin: true,
        headers: {
            Connection: 'Keep-Alive'
        }
    } as Options);

    app.use(appProxy);
};
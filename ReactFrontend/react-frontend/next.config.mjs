/** @type {import('next').NextConfig} */
const nextConfig = {
    output: 'export',
    images: { unoptimized: true },
    experimental: {
        instrumentationHook: true
    }
    
    // FOR output: 'standalone'
    // rewrites: async () => {
    //     if (process.env["services__apiservice__https__0"]) 
    //         return [{
    //             source: "/api/:path*",
    //             destination: `${process.env["services__apiservice__http__0"]}/:path*`
    //     }];
    //     if (process.env["services__apiservice__http__0"]) 
    //         return [{
    //             source: "/api/:path*",
    //             destination: `${process.env["services__apiservice__http__0"]}/:path*`
    //     }];
    //     return [];
    // }
};

export default nextConfig;

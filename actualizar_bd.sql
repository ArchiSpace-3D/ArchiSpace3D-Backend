-- Script idempotente de actualización de Base de Datos (ArchiSpace3D)
-- Puede ejecutarse múltiples veces en pgAdmin / Supabase / Railway sin errores.

-- 1. Cómputos Métricos en medicion
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS etapa character varying;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS partida character varying;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS descripcion character varying;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS veces integer DEFAULT 1;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS largo numeric;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS ancho numeric;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS alto numeric;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS unidad character varying;
ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS totalparcial numeric;

-- 2. Columnas nuevas en proyecto, usuario y versiondiseno
ALTER TABLE public.proyecto ADD COLUMN IF NOT EXISTS imagenurl character varying(1024);
ALTER TABLE public.proyecto ADD COLUMN IF NOT EXISTS codigosalaactiva character varying(20);
ALTER TABLE public.proyecto ADD COLUMN IF NOT EXISTS fechaaperturasala timestamp without time zone;
ALTER TABLE public.usuario ADD COLUMN IF NOT EXISTS avatar_url character varying(1024);
ALTER TABLE public.usuario ADD COLUMN IF NOT EXISTS fcmtoken character varying(512);
ALTER TABLE public.versiondiseno ADD COLUMN IF NOT EXISTS puntosreferencia jsonb DEFAULT '{}'::jsonb;

-- 3. Permitir múltiples espacios físicos por proyecto (inventario de ambientes + plano CAD)
ALTER TABLE public.espaciofisico DROP CONSTRAINT IF EXISTS espaciofisico_idproyecto_key;
DROP INDEX IF EXISTS public.espaciofisico_idproyecto_key;

-- 4. Sistema de Sugerencias
CREATE TABLE IF NOT EXISTS public.sugerencia (
    idsugerencia serial PRIMARY KEY,
    idproyecto integer NOT NULL REFERENCES public.proyecto(idproyecto) ON DELETE CASCADE,
    idusuario integer NOT NULL REFERENCES public.usuario(idusuario) ON DELETE CASCADE,
    titulo character varying(255) NOT NULL,
    descripcion text NOT NULL,
    estado character varying(50) DEFAULT 'Pendiente',
    fechacreacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);

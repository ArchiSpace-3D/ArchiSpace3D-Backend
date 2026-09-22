-- Actualización de la tabla medicion para soportar Cómputos Métricos
-- Instrucciones: Ejecuta este script en pgAdmin o en la terminal SQL de tu base de datos.

ALTER TABLE public.medicion
ADD COLUMN etapa character varying,
ADD COLUMN partida character varying,
ADD COLUMN descripcion character varying,
ADD COLUMN veces integer DEFAULT 1,
ADD COLUMN largo numeric,
ADD COLUMN ancho numeric,
ADD COLUMN alto numeric,
ADD COLUMN unidad character varying,
ADD COLUMN totalparcial numeric;
-- Sistema de Sugerencias
CREATE TABLE public.sugerencia (
    idsugerencia serial PRIMARY KEY,
    idproyecto integer NOT NULL REFERENCES public.proyecto(idproyecto) ON DELETE CASCADE,
    idusuario integer NOT NULL REFERENCES public.usuario(idusuario) ON DELETE CASCADE,
    titulo character varying(255) NOT NULL,
    descripcion text NOT NULL,
    estado character varying(50) DEFAULT 'Pendiente',
    fechacreacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);

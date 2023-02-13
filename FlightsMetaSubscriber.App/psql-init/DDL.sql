create table users
(
    id       bigint not null
        constraint user_pk
            primary key,
    username varchar,
    active   boolean
);

alter table users
    owner to postgres;

create table subscription
(
    id                 serial
        constraint subscription_pk
            primary key,
    user_id            bigint
        constraint subscription_users_id_fk
            references users,
    departure_min_date date,
    departure_max_date date,
    only_direct        boolean,
    active             boolean
);

alter table subscription
    owner to postgres;

create table origin
(
    subscribe_id integer not null
        constraint origin_subscription_id_fk
            references subscription,
    user_id      bigint  not null,
    iata_code    varchar not null,
    iata_name    varchar,
    constraint origin_pk
        primary key (subscribe_id, user_id, iata_code)
);

alter table origin
    owner to postgres;

create table destination
(
    subscribe_id integer not null
        constraint destination_subscription_id_fk
            references subscription,
    user_id      bigint  not null,
    iata_code    varchar not null,
    iata_name    varchar,
    constraint destination_pk
        primary key (subscribe_id, user_id, iata_code)
);

alter table destination
    owner to postgres;

create table search_result
(
    id                serial
        constraint search_result_pk
            primary key,
    subscription_id   integer   not null
        constraint search_result_subscription_id_fk
            references subscription,
    search_dt         timestamp not null,
    origin            varchar   not null,
    destination       varchar   not null,
    departure_at      timestamp,
    price             numeric,
    ticket_link       varchar,
    dt_offset         varchar,
    number_of_changes integer
);

alter table search_result
    owner to postgres;
